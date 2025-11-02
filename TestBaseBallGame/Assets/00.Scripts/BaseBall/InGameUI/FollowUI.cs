using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FollowUI : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Setup")]
    [SerializeField]
    private Canvas _canvas;            // 비워두면 상위에서 자동 탐색
    [SerializeField] 
    private bool _blockRaycastsWhileDragging = false; // 드래그 중 다른 UI 클릭 허용 여부
    [SerializeField] 
    private bool _returnToStartIfNoDrop = false;      // 드롭 타겟 없으면 원래 자리로 복귀

    [Header("Events")]
    public UnityEvent onPressed;                       // 누를 때
    public UnityEvent onBeginDrag;                     // 드래그 시작
    public Vector2Event onDragging;                    // 드래그 중 (현재 anchoredPosition)
    public UnityEvent onReleased;                      // 뗄 때(클릭업)
    public UnityEvent onEndDrag;                       // 드래그 종료(성공/실패 포함)
    public GameObjectEvent onDroppedOn;                // 드롭된 대상(없으면 null)

    [System.Serializable] 
    public class Vector2Event : UnityEvent<Vector2> { }
    [System.Serializable] 
    public class GameObjectEvent : UnityEvent<GameObject> { }

    private RectTransform _rect;
    private CanvasGroup _cg;
    private Vector2 _startAnchoredPos;
    private Vector2 _pointerOffsetLocal; // 마우스 포인터와 요소 좌표의 오프셋

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        if (!_canvas) _canvas = GetComponentInParent<Canvas>();
        _cg = GetComponent<CanvasGroup>();
        if (!_cg) _cg = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPressed?.Invoke();
        CachePointerOffset(eventData);
        _startAnchoredPos = _rect.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag?.Invoke();
        _cg.blocksRaycasts = _blockRaycastsWhileDragging;  // false면 드래그 중 하위가 드롭 타겟으로 히트됨
        CachePointerOffset(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_canvas) return;

        RectTransform parentRt = _rect.parent as RectTransform;
        if (!parentRt) return;

        // 스크린 포인트를 부모 로컬 좌표로 변환
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRt, eventData.position, _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out var local))
        {
            _rect.anchoredPosition = local - _pointerOffsetLocal;
            onDragging?.Invoke(_rect.anchoredPosition);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드롭 타겟 찾기
        GameObject dropTarget = FindDropTarget(eventData);

        // 사용자 이벤트(드롭된 오브젝트 넘겨줌; 없으면 null)
        onDroppedOn?.Invoke(dropTarget);

        // 표준 IDropHandler에도 메시지 전달 (있다면)
        if (dropTarget)
        {
            ExecuteEvents.ExecuteHierarchy(dropTarget, eventData, ExecuteEvents.dropHandler);
        }
        else if (_returnToStartIfNoDrop)
        {
            _rect.anchoredPosition = _startAnchoredPos;
        }

        _cg.blocksRaycasts = true;
        onEndDrag?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onReleased?.Invoke();
    }

    private void CachePointerOffset(PointerEventData eventData)
    {
        if (!_canvas) return;
        RectTransform parentRt = _rect.parent as RectTransform;
        if (!parentRt) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRt, eventData.position, _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out var local))
        {
            // 포인터가 박스 내부 어디를 잡았는지 오프셋 기억
            _pointerOffsetLocal = local - _rect.anchoredPosition;
        }
    }

    private GameObject FindDropTarget(PointerEventData eventData)
    {
        // 현재 포인터 위치로 UI 레이캐스트
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var r in results)
        {
            // 1) 커스텀 IUIDropTarget 먼저
            if (r.gameObject.TryGetComponent<IUIDropTarget>(out var custom))
            {
                custom.OnUIDrop(gameObject);
                return r.gameObject;
            }

            // 2) 표준 IDropHandler가 있으면 그것도 타깃으로 인정
            if (ExecuteEvents.CanHandleEvent<IDropHandler>(r.gameObject))
            {
                return r.gameObject;
            }
        }
        return null;
    }
}

/// <summary>커스텀 드롭 타겟용 인터페이스(원하면 사용)</summary>
public interface IUIDropTarget
{
    void OnUIDrop(GameObject dropped);
}

