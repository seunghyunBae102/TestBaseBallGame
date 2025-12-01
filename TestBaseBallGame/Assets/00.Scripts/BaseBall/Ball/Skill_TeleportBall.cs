using UnityEngine;
using Bash.Framework.Core;
using Bash.Core.GamePlay;
using Bash.Core.Ball;

public class Skill_TeleportBall : ActorCompo
{
    [Header("Skill Settings")]
    [SerializeField][Range(0f, 1f)] private float _activationChance = 1.0f; // 100% 발동

    // 투수 컴포넌트를 오버라이드하거나, 투구 직전에 전략을 바꿔치기 해야 함.
    // 여기서는 PitcherCompo가 이 스킬의 존재를 확인하고 전략을 가져다 쓰는 구조를 제안하거나,
    // 더 유연하게는 '이벤트'를 통해 전략을 주입합니다.

    // 가장 쉬운 방법: PitcherCompo 수정 없이 구현하기 위해
    // PitcherCompo가 'DoPitch' 할 때 Strategy를 외부에서 주입받을 수 있게 열어주는 것이 좋습니다.
    // 하지만 지금은 예제이므로, 투수 객체의 메서드를 덮어쓰는 대신
    // 투수가 공을 만든 직후(PitchStart) 공의 전략을 훔쳐서 바꿔버립니다.

    // *이 구현을 위해 PitcherCompo에서 공 생성 후 이벤트를 하나 쏴주면 좋습니다.*
    // 여기서는 간단히 PitcherCompo를 참조해서 직접 제어하는 방식(데코레이터)을 씁니다.

    private PitcherCompo _pitcher;

    protected override void OnInit()
    {
        _pitcher = Owner.GetCompo<PitcherCompo>();
    }

    // 외부에서 "스킬샷 발사" 버튼을 누르면 호출
    public void CastTeleportShot(Vector3 target)
    {
        if (_pitcher == null) return;

        Debug.Log("[Skill] Teleport Shot Activated!");

        // 1. 투수에게 일단 던지게 시킴 (공 생성됨)
        _pitcher.DoPitch(target);

        // 2. 현재 날아가는 공을 찾아서 전략 강제 교체
        // (PitcherCompo에 CurrentBall 프로퍼티가 public이어야 함)
        var ball = GameRoot.Instance.FindNode<BallNode>(); // 혹은 _pitcher.CurrentBall
        if (ball != null)
        {
            var phys = ball.GetCompo<BallPhysicsCompo>();

            // 순간이동 전략 주입!
            phys.SetStrategy(new TeleportStrategy(ball.transform.position, target, 40.0f));
        }
    }
}