
public class StunReceiverModule : Module
{
    private float _stunTimer;

    protected override void OnInit()
    {
        base.OnInit();
        Events.Subscribe<SkillUsedEvent>(OnSkillUsed);
    }

    protected override void OnShutdown()
    {
        base.OnShutdown();
        Events.Unsubscribe<SkillUsedEvent>(OnSkillUsed);
    }

    protected override void Tick(float dt)
    {
        if (_stunTimer > 0f)
        {
            _stunTimer -= dt;
            // 여기에서 입력 막기, 애니메이션 재생 등
        }
    }

    private void OnSkillUsed(SkillUsedEvent e)
    {
        // 이 노드가 타겟인지 판별하는 로직은 나중에 추가
        _stunTimer = Mathf.Max(_stunTimer, e.StunDuration);
    }
}
