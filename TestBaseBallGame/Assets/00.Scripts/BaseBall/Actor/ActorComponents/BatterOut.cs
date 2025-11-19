using UnityEngine;

public class BatterOut : ActorComponent
{
    protected bool _bIsSafe;
    protected int _strikecnt;

    public bool IsSafe
    {
        get => _bIsSafe;
        set => _bIsSafe = value;
    }

    
}
