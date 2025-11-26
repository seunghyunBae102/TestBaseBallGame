// Scripts/Game/Baseball/Runner/RunnerRuleModule.cs
using UnityEngine;

using Bash.Framework.Core;
using Bash.Game.Baseball.Shared;
using Bash.Game.Baseball.Events;

namespace Bash.Game.Baseball.Runner
{
    /// <summary>
    /// 타구 결과(HitBallResult)를 받아서
    /// - RunnerAdvanceRequest
    /// - RunnerOutEvent
    /// - AtBatEndedEvent
    /// 를 생성하는 모듈.
    /// 실제 베이스 상태 변경은 RunnerStateModule이 담당한다.
    /// </summary>
    public class RunnerRuleModule : Module
    {
        [Header("Config")]
        [Tooltip("한 팀의 타자 수(타순 순환용).")]
        [SerializeField] private int battingOrderSize = 9;

        private TeamSide _offenseTeam;
        private RunnerId _currentBatter;
        private int _currentBattingOrderIndex;
        private bool _hasCurrentBatter;

        protected override void OnInit()
        {
            var events = GameRoot.Instance.Events;
            events.Subscribe<InningHalfStarted>(OnInningHalfStarted);
            events.Subscribe<AtBatStarted>(OnAtBatStarted);
            events.Subscribe<HitBallResult>(OnHitBallResult);
        }

        protected override void OnShutdown()
        {
            var events = GameRoot.Instance.Events;
            events.Unsubscribe<InningHalfStartedEvent>(OnInningHalfStarted);
            events.Unsubscribe<AtBatStarted>(OnAtBatStarted);
            events.Unsubscribe<HitBallResult>(OnHitBallResult);
        }

        private void OnInningHalfStarted(InningHalfStarted e)
        {
            _offenseTeam = e.OffenseTeam;
            _currentBattingOrderIndex = 0;
            _hasCurrentBatter = false;
        }

        private void OnAtBatStarted(AtBatStarted e)
        {
            _offenseTeam = e.OffenseTeam;
            _currentBatter = e.Batter;
            _currentBattingOrderIndex = e.BattingOrderIndex;
            _hasCurrentBatter = true;
        }

        private void OnHitBallResult(HitBallResult result)
        {
            if (!_hasCurrentBatter)
            {
                Debug.LogWarning("[RunnerRule] 현재 타자 정보가 없습니다.");
                return;
            }

            if (!result.IsFair && result.HitType == HitType.FoulBall)
            {
                // 순수 파울 타구는 주루 변화 없음 (카운트만 변화).
                return;
            }

            var events = GameRoot.Instance.Events;

            // 1) 공중에서 잡힌 경우: 타자 플라이 아웃, AtBatEnded 발행.
            if (result.IsCaught)
            {
                var outEv = new RunnerOutEvent
                {
                    Runner = _currentBatter,
                    AtBase = BaseIndex.Home,
                    Reason = OutReason.FlyOut
                };
                events.Publish(outEv);

                var endEv = new AtBatEndedEvent
                {
                    OffenseTeam = _offenseTeam,
                    Batter = _currentBatter,
                    Reason = AtBatEndReason.SacrificeFly // 단순화된 처리
                };
                events.Publish(endEv);
                return;
            }

            // 2) 홈런 – 모든 주자+타자가 득점.
            if (result.IsHomeRun)
            {
                PublishHomeRunAdvances();
                var endEv = new AtBatEndedEvent
                {
                    OffenseTeam = _offenseTeam,
                    Batter = _currentBatter,
                    Reason = AtBatEndReason.HomeRun
                };
                events.Publish(endEv);
                return;
            }

            // 3) 일반 안타 – BaseCount에 따라 단타/2루타/3루타 처리.
            if (!result.IsFair)
            {
                // 여기로 들어올 일은 거의 없지만, 일단 안전 처리.
                return;
            }

            int baseCount = Mathf.Clamp(result.BaseCount, 1, 4);
            PublishHitAdvances(baseCount);

            // 안타 후 타석 종료
            {
                var endEv = new AtBatEndedEvent
                {
                    OffenseTeam = _offenseTeam,
                    Batter = _currentBatter,
                    Reason = AtBatEndReason.HitInPlay
                };
                events.Publish(endEv);
            }
        }

        /// <summary>홈런일 때 주자와 타자 모두 득점하도록 AdvanceRequest들을 발행.</summary>
        private void PublishHomeRunAdvances()
        {
            var events = GameRoot.Instance.Events;

            // 3루 주자: +1
            events.Publish(new RunnerAdvanceRequest
            {
                Runner = default, // 실제 주자는 RunnerStateModule이 필터링해서 처리
                FromBase = BaseIndex.Third,
                AdvanceCount = 1,
                Forced = false
            });

            // 2루 주자: +2
            events.Publish(new RunnerAdvanceRequest
            {
                Runner = default,
                FromBase = BaseIndex.Second,
                AdvanceCount = 2,
                Forced = false
            });

            // 1루 주자: +3
            events.Publish(new RunnerAdvanceRequest
            {
                Runner = default,
                FromBase = BaseIndex.First,
                AdvanceCount = 3,
                Forced = false
            });

            // 타자: 홈에서 +4
            events.Publish(new RunnerAdvanceRequest
            {
                Runner = _currentBatter,
                FromBase = BaseIndex.Home,
                AdvanceCount = 4,
                Forced = false
            });
        }

        /// <summary>
        /// 일반 안타일 때, baseCount(단타/2루타/3루타)에 따라
        /// 러너와 타자의 AdvanceRequest를 발행.
        /// </summary>
        private void PublishHitAdvances(int baseCount)
        {
            var events = GameRoot.Instance.Events;

            // 단순 기본 룰:
            // - 단타: 3루->홈, 2루->3루, 1루->2루, 타자->1루
            // - 2루타: 3루->홈, 2루->홈, 1루->3루, 타자->2루
            // - 3루타: (단순화) 3루->홈, 2루->홈, 1루->홈, 타자->3루

            int adv3 = 0;
            int adv2 = 0;
            int adv1 = 0;
            int advB = baseCount;

            switch (baseCount)
            {
                case 1:
                    adv3 = 1;
                    adv2 = 1;
                    adv1 = 1;
                    advB = 1;
                    break;
                case 2:
                    adv3 = 1;
                    adv2 = 2;
                    adv1 = 2;
                    advB = 2;
                    break;
                case 3:
                    adv3 = 1;
                    adv2 = 3;
                    adv1 = 3;
                    advB = 3;
                    break;
                default:
                    adv3 = 1;
                    adv2 = 1;
                    adv1 = 1;
                    advB = baseCount;
                    break;
            }

            // 3루, 2루, 1루 순서로 발행해야 포스플레이 충돌을 줄인다.
            if (adv3 > 0)
            {
                events.Publish(new RunnerAdvanceRequest
                {
                    Runner = default,          // 실제 러너는 RunnerState가 찾을 것
                    FromBase = BaseIndex.Third,
                    AdvanceCount = adv3,
                    Forced = true
                });
            }

            if (adv2 > 0)
            {
                events.Publish(new RunnerAdvanceRequest
                {
                    Runner = default,
                    FromBase = BaseIndex.Second,
                    AdvanceCount = adv2,
                    Forced = true
                });
            }

            if (adv1 > 0)
            {
                events.Publish(new RunnerAdvanceRequest
                {
                    Runner = default,
                    FromBase = BaseIndex.First,
                    AdvanceCount = adv1,
                    Forced = true
                });
            }

            if (advB > 0)
            {
                events.Publish(new RunnerAdvanceRequest
                {
                    Runner = _currentBatter,
                    FromBase = BaseIndex.Home,
                    AdvanceCount = advB,
                    Forced = true
                });
            }
        }
    }
}
