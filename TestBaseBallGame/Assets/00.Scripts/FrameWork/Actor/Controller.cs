using UnityEngine;

public class Controller : Actor
{
    protected Pawn _pawn;

    protected override void Awake()
    {
        _pawn = GetComponent<Pawn>();
        if (_pawn != null)
        {
            _pawn.SetController(this);
        }
    }
}
