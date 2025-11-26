using UnityEngine;

public abstract class BallBehaviourSO : ScriptableConfig, IBallBehaviour
{
    public virtual void Initialize(ref BallState state, in BallContext ctx) { }

    public abstract void SimulateStep(ref BallState state, in BallContext ctx, float dt);
}
