

[CreateAssetMenu(menuName = "Baseball/BallBehaviour/Hit/Fly")]
public class FlyBallBehaviourSO : BallBehaviourSO
{
    public float drag = 0.03f;

    public override void Initialize(ref BallState state, in BallContext ctx)
    {
        state.isAlive = true;
    }

    public override void SimulateStep(ref BallState state, in BallContext ctx, float dt)
    {
        state.velocity += ctx.gravity * dt;
        state.velocity *= (1f - drag * dt);
        state.position += state.velocity * dt;
    }
}
    
