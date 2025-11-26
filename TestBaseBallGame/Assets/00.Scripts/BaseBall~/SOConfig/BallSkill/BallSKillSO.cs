public abstract class BallSkillSO : ScriptableConfig, IBallStepModifier
{
    public abstract void Modify(ref BallState state, in BallContext ctx, float dt);
}
