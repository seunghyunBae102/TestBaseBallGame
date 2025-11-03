using UnityEngine;

public class PitcherController : Controller
{
    private Pawn _pitcher;
    private PawnMovement _movement;
    private ISKILLable _skillSystem;
    
    [SerializeField]
    private Transform _throwPoint;
    
    private bool _isReadyToThrow;
    private float _currentPitchDelay;
    
    protected virtual void Awake()
    {
        _pitcher = GetComponent<Pawn>();
        _movement = GetComponent<PawnMovement>();
        _skillSystem = GetComponent<ISKILLable>();
    }

    public void PreparePitch()
    {
        _isReadyToThrow = true;
        // 투구 준비 애니메이션 실행
    }

    public void ExecutePitch(Vector3 targetPosition, float power)
    {
        if (!_isReadyToThrow) return;
        
        // 투구 스킬 실행
        if (_skillSystem != null)
        {
            _skillSystem.ExecuteSkill("Pitch", new object[] { targetPosition, power });
        }
        
        _isReadyToThrow = false;
    }

    public void MoveToPitchingPosition(Vector3 position)
    {
        Vector2 moveDir = new Vector2(position.x - transform.position.x, position.z - transform.position.z).normalized;
        _movement.SetMovementInput(moveDir);
    }
}
