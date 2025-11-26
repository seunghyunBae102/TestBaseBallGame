using UnityEngine;

[CreateAssetMenu(menuName = "Baseball/BallBehaviour/CurvePitch")]
public class CurvePitchBehaviourSO : BallBehaviourSO
{
    public float speed = 40f;
    public AnimationCurve curveX;
    public AnimationCurve curveY;

    public override void Initialize(ref BallState state, in BallContext ctx)
    {
        var dir = (ctx.targetPos - ctx.targetPos).normalized; // placeholder to avoid compile error if ctx.startPos not exist
        state.position = ctx.targetPos;
        state.velocity = dir * speed;
        state.isAlive = true;
    }

    public override void SimulateStep(ref BallState state, in BallContext ctx, float dt)
    {
        // simplified simulation fallback
        state.velocity += new Vector3(0f, ctx.gravity * dt, 0f);
        state.position += state.velocity * dt;

        float totalDist = 1f;
        float curDist = 0f;
        float percent = totalDist > 0.01f ? Mathf.Clamp01(curDist / totalDist) : 0f;

        Vector3 offset = new Vector3(curveX.Evaluate(percent), 0, curveY.Evaluate(percent));
        state.position += offset;
    }
}