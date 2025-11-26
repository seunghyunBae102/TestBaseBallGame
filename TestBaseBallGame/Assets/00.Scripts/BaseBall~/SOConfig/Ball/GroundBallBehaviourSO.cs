
[CreateAssetMenu(menuName = "Baseball/BallBehaviour/Hit/Ground")]
public class GroundBallBehaviourSO : BallBehaviourSO
{
    public float groundFriction = 5f;

    public override void Initialize(ref BallState state, in BallContext ctx)
    {
        state.isAlive = true;
    }

    public override void SimulateStep(ref BallState state, in BallContext ctx, float dt)
    {
        // ���� �ִٰ� �����ϸ�, �߷��� ���� �����ϰ� ���� �ӵ��� ����
        Vector3 horizontal = new Vector3(state.velocity.x, 0, state.velocity.z);
        float speed = horizontal.magnitude;
        float dec = groundFriction * dt;
        speed = Mathf.Max(0, speed - dec);

        if (speed > 0)
            horizontal = horizontal.normalized * speed;
        else
            horizontal = Vector3.zero;

        state.velocity = new Vector3(horizontal.x, 0, horizontal.z);
        state.position += state.velocity * dt;
    }
}

