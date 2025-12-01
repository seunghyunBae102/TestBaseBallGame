using UnityEngine;
using Bash.Framework.Core;
using Bash.Framework.Core.Events;

public class Skill_TheJoker : ActorCompo
{
    [Header("Settings")]
    [SerializeField] private float _triggerChance = 0.1f; // 10%

    protected override void OnInit()
    {
        // 심판이 판정 내리기 직전 이벤트를 구독
        Events.Subscribe<PreJudgementEvent>(OnPreJudge);
    }

    protected override void OnDestroy()
    {
        Events.Unsubscribe<PreJudgementEvent>(OnPreJudge);
    }

    private void OnPreJudge(PreJudgementEvent evt)
    {
        // 이미 다른 스킬이 건드렸다면 패스 (우선순위 처리 가능)
        if (evt.IsModified) return;

        // "스트라이크" 판정이 나올 때만 발동
        if (evt.OriginalResult == JudgementEvent.Type.Strike)
        {
            // 10% 확률 체크
            if (Random.value < _triggerChance)
            {
                // 판정 뒤집기!
                evt.FinalResult = JudgementEvent.Type.Out;
                evt.IsModified = true;

                Debug.Log($"<color=magenta>[Skill] JOKER ACTIVATED! Strike -> OUT forced.</color>");

                // (선택) 스킬 발동 사운드나 이펙트 재생 요청
                // GameRoot.Instance.GetManager<SoundManager>().PlaySFX(...);
            }
        }
    }
}