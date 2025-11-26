// Scripts/Game/Baseball/Ball/Config/StraightPitchBehaviourSO.cs

using UnityEngine;


    [CreateAssetMenu(menuName = "Baseball/BallBehaviour/StraightPitch")]
    public class StraightPitchBehaviourSO : BallBehaviourSO
    {
        public float speed = 40f;      // m/s ����
        public float drag = 0.1f;

        public override void Initialize(ref BallState state, in BallContext ctx)
        {
            Vector3 dir = (ctx.targetPos - ctx.startPos).normalized;
            state.velocity = dir * speed;
            state.isAlive = true;
        }

        public override void SimulateStep(ref BallState state, in BallContext ctx, float dt)
        {
            // ������ �߷� + �巡��
            state.velocity += ctx.gravity * dt;
            state.velocity *= (1f - drag * dt);

            state.position += state.velocity * dt;
            state.time += dt;
        }
    }


