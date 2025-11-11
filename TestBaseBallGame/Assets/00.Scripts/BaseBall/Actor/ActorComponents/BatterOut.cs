using UnityEngine;

public class BatterOut : ActorComponent
{
    protected bool _bIsSafe;

    public bool IsSafe
    {
        get => _bIsSafe;
        set => _bIsSafe = value;
    }

    
}
