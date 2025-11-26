using UnityEngine;

public class SkillModule : Module
{
    [SerializeField] private SkillConfig _config;

    private float _cooldownTimer;

    protected override void Tick(float dt)
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= dt;

        // 디버그 테스트용: 스페이스 키로 스킬 발동
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryUseSkill();
        }
    }

    public bool TryUseSkill()
    {
        if (_cooldownTimer > 0f) return false;

        _cooldownTimer = _config.cooldown;

        // 여기서 이벤트 발행 (예: "이 스킬로 상대를 스턴시켰다")
        Events.Publish(new SkillUsedEvent
        {
            SkillId = _config.Id,
            Power = _config.power,
            StunDuration = _config.stunDuration,
            Owner = Owner
        });

        return true;
    }
}

public struct SkillUsedEvent
{
    public string SkillId;
    public float Power;
    public float StunDuration;
    public BashNode Owner;
}
