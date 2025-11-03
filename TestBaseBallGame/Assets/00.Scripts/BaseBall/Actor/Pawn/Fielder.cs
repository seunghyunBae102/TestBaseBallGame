using JetBrains.Annotations;
using UnityEngine;


[RequireComponent(typeof(PawnMovement))]
public class Fielder : Pawn, IBallHitable
{
    public PawnMovement MovementCompo;


    public void InitCharacter()
    {

    }

    public void MovementInput(Vector3 dir)
    {
        MovementCompo.SetMovementInput(dir);
    }

    public void OnBallHit(Vector3 hitPoint, Vector3 hitNormal)
    {
        throw new System.NotImplementedException();
    }

    public void OnHitByBall(HitedBall ball, RaycastHit hit) //잡는 기능도 분리 하는 게 좋으려나?
    {
        
    }
}
