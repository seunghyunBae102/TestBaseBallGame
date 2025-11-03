using UnityEngine;

using UnityEngine.UI;

public class BattingUIController : MonoBehaviour
{
    [SerializeField] private FollowUI _followUI;
    [SerializeField] private RectTransform _battingArea; // 타격 가능 영역
    [SerializeField] private Image _powerGauge; // 선택사항: 파워 게이지 UI
    
    private IBattingControl _batterController;
    private bool _isDragging;
    
    private void Awake()
    {
        // BatterController 찾기
        //_batterController = FindObjectOfType<BatterController>();
        
        // FollowUI 이벤트 연결
        if (_followUI != null)
        {
            _followUI.onPressed.AddListener(OnBattingStart);
            _followUI.onDragging.AddListener(OnBattingDrag);
            _followUI.onEndDrag.AddListener(OnBattingComplete);
            _followUI.onReleased.AddListener(OnBattingReleased);
        }
    }
    
    private void OnBattingStart()
    {
        if (_batterController?.CanBat != true) return;
        
        _isDragging = true;
        _batterController.OnBattingStart(_followUI.transform.position);
        
        // UI 피드백
        if (_powerGauge != null)
            _powerGauge.fillAmount = 0f;
    }
    
    private void OnBattingDrag(Vector2 position)
    {
        if (!_isDragging || _batterController?.CanBat != true) return;
        
        // 드래그 위치가 유효한 영역인지 확인
        if (_battingArea != null && !RectTransformUtility.RectangleContainsScreenPoint(_battingArea, position))
            return;
            
        _batterController.OnBattingDrag(position);
        
        // UI 피드백 업데이트
        if (_powerGauge != null)
        {
            float distance = Vector2.Distance(_followUI.transform.position, position);
            _powerGauge.fillAmount = distance / 300f; // maxDragDistance와 동일한 값 사용
        }
    }
    
    private void OnBattingComplete()
    {
        if (!_isDragging || _batterController?.CanBat != true) return;
        
        _batterController.OnBattingComplete(_followUI.transform.position);
        _isDragging = false;
        
        // UI 리셋
        if (_powerGauge != null)
            _powerGauge.fillAmount = 0f;
    }
    
    private void OnBattingReleased()
    {
        _isDragging = false;
    }
}