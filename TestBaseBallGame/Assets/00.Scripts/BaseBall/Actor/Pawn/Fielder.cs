using JetBrains.Annotations;
using UnityEngine;


[RequireComponent(typeof(PawnMovement))]
public class Fielder : Pawn, IBallHitable
{
    private PawnMovement _movement;
    private PawnPathFind _pathFinder;

    protected override void Awake()
    {
        base.Awake();
        _movement = GetComponent<PawnMovement>();
        _pathFinder = GetComponent<PawnPathFind>();
    }

    public void InitCharacter()
    {

    }

    public void MovementInput(Vector3 dir)
    {
        _movement.SetMovementInput(dir);
    }

    public void MoveToPosition(Vector3 position)
    {
 
        if (_movement != null)
        {
            Vector3 moveDir = Vector3.zero;
            if (_pathFinder != null)
            {
                moveDir = _pathFinder.PathFind(position);

            }
            _movement.SetMovementInput(moveDir);
        }
    }

    public void OnBallHit(Vector3 hitPoint, Vector3 hitNormal)
    {
        throw new System.NotImplementedException();
    }

    public void OnHitByBall(HitedBall ball, RaycastHit hit) //잡는 기능도 분리 하는 게 좋으려나?
    {
        
    }

    public void CatchBall()
    {
        // 공 잡기 구현
    }

    public void ThrowBall(Vector3 targetPosition)
    {
        // 공 던지기 구현
    }
}
