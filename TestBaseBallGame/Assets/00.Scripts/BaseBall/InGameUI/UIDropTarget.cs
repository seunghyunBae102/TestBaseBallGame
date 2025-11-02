using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class UIDropTarget : MonoBehaviour, IUIDropTarget, IDropHandler
{
    [Header("Events")]
    public GameObjectEvent onReceived;  // 이 타겟 위에 드롭된 오브젝트
    [System.Serializable] public class GameObjectEvent : UnityEvent<GameObject> { }

    // 커스텀 인터페이스 경로
    public virtual void OnUIDrop(GameObject dropped)
    {
        onReceived?.Invoke(dropped);
        // TODO: 여기서 원하는 로직 (예: 부모 바꾸기, 스냅 등)
    }

    // 표준 이벤트 시스템 경로
    public virtual void OnDrop(PointerEventData eventData)
    {
        //var go = eventData.pointerDrag;
        //if (go) onReceived?.Invoke(go);
    }
}
