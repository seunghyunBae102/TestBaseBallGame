using UnityEngine;

public class Pitcher : Pawn
{
    //private PawnMovement _movement;
    private ISKILLable _skillSystem;
    
    [SerializeField] private Transform _throwPoint;
    private bool _isReadyToThrow;
    
    protected override void Awake()
    {
        base.Awake();
        //_movement = GetComponent<PawnMovement>();
        _skillSystem = GetComponent<ISKILLable>();
    }

    public void PreparePitch()
    {
        _isReadyToThrow = true;
    }

    public void ExecutePitch(Vector3 targetPosition, float power)
    {
        if (!_isReadyToThrow) return;
        _skillSystem?.ExecuteSkill("Pitch", new object[] { targetPosition, power });
        _isReadyToThrow = false;
    }

    //public void MoveToPosition(Vector3 position)
    //{
    //    if (_movement == null) return;
    //    Vector2 moveDir = new Vector2(
    //        position.x - transform.position.x,
    //        position.z - transform.position.z).normalized;
    //    _movement.SetMovementInput(moveDir);
    //}
}
