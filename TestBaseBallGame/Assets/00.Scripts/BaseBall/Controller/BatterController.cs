using UnityEngine;

public class BatterController : Controller, IBattingControl
{
    private Pawn _batter;
    private Batting _battingSystem;
    private PawnMovement _movement;
    
    [SerializeField] private Transform _batPoint;
    [SerializeField] private float _maxBattingPower = 100f;
    [SerializeField] private float _minBattingPower = 10f;
    
    private bool _isReadyToBat;
    private Vector2 _battingStartPos;
    private float _currentBattingPower;
    
    public bool CanBat => _isReadyToBat && _battingSystem != null;
    
    protected virtual void Awake()
    {
        _batter = GetComponent<Pawn>();
        _battingSystem = GetComponent<Batting>();
        _movement = GetComponent<PawnMovement>();
    }

    public void OnBattingStart(Vector2 position)
    {
        if (!CanBat) return;
        _battingStartPos = position;
        _currentBattingPower = _minBattingPower;
    }

    public void OnBattingDrag(Vector2 position)
    {
        if (!CanBat) return;
        
        // 드래그 거리에 따른 파워 계산
        float distance = Vector2.Distance(_battingStartPos, position);
        _currentBattingPower = Mathf.Lerp(_minBattingPower, _maxBattingPower, 
            distance / Screen.height);
    }

    public void OnBattingComplete(Vector2 position)
    {
        if (!CanBat) return;
        
        Vector3 targetPoint = CalculateHitTarget(position);
        ExecuteSwing(targetPoint, _currentBattingPower);
    }

    private Vector3 CalculateHitTarget(Vector2 screenPosition)
    {
        // 스크린 좌표를 월드 좌표로 변환
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        
        if (groundPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        
        return transform.position + transform.forward * 10f;
    }

    public void PrepareBat()
    {
        _isReadyToBat = true;
        // 타격 준비 애니메이션
    }

    public void ExecuteSwing(Vector3 targetPoint, float power)
    {
        if (!_isReadyToBat) return;
        
        if (_battingSystem != null)
        {
            _battingSystem.Swing(targetPoint, power);
        }
        
        _isReadyToBat = false;
    }

    public void MoveToHomeBase(Vector3 position)
    {
        Vector2 moveDir = new Vector2(position.x - transform.position.x, position.z - transform.position.z).normalized;
        _movement.SetMovementInput(moveDir);
    }
}
