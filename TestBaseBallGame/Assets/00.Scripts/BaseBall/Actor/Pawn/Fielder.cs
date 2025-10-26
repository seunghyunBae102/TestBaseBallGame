using JetBrains.Annotations;
using UnityEngine;


[RequireComponent(typeof(PawnMovement))]
public class Fielder : Pawn
{
    public PawnMovement MovementCompo;


    public void InitCharacter()
    {

    }

    public void MovementInput(Vector3 dir)
    {
        MovementCompo.SetMovementInput(dir);
    }




}
