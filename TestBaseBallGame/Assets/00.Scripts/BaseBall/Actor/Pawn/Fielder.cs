using JetBrains.Annotations;
using UnityEngine;


[RequireComponent(typeof(PawnMovement))]
public class Fielder : Pawn,IBallHitable
{
    public PawnMovement MovementCompo;


    public void InitCharacter()
    {

    }

    public void MovementInput(Vector3 dir)
    {
        MovementCompo.SetMovementInput(dir);
    }

    public void OnHitByBall(HitedBall ball, RaycastHit hit) //��� ��ɵ� �и� �ϴ� �� ��������?
    {
        throw new System.NotImplementedException();
    }
}
