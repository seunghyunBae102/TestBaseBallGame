// Scripts/Game/Baseball/State/CountStateModule.cs
using UnityEngine;
using Bash.Framework.Core;
using Bash.Framework.Core;
using Bash.Game.Baseball.Config;
using Bash.Game.Baseball.Shared;
using Bash.Game.Baseball.Events;

namespace Bash.Game.Baseball.State
{
    /// <summary>
    /// 볼/스트라이크/아웃 카운트를 관리하는 상태 모듈.
    /// - GameModeConfig 설정에 따라 기본 야구 규칙을 적용.
    /// - PitchOutcome, BatterAwardedBase, BatterOut, InningHalfStarted 등의 이벤트를 처리한다.
    /// - CountChanged, BatterAwardedBase(볼넷/사구), BatterOut(삼진), AtBatEnded 등을 발행한다.
    /// </summary>
    public class CountStateModule : Module
    {
        [Header("Config")]
        [SerializeField] private GameModeConfig gameModeConfig;

        public int Balls { get; private set; }
        public int Strikes { get; private set; }
        public int Outs { get; private set; }

        /// <summary>
        /// 외부에서 필요할 때 카운트를 리셋할 수 있는 헬퍼.
        /// (타석 리셋: 볼/스트라이크만 0으로)
        /// </summary>
        public void ResetCount()
        {
            Balls = 0;
            Strikes = 0;
            PublishCountChanged();
        }

        /// <summary>
        /// 이닝 전체 리셋 (볼/스트/아웃 모두).
        /// 보통 새 이닝 시작 시점에 사용.
        /// </summary>
        public void ResetAll()
        {
            Balls = 0;
            Strikes = 0;
            Outs = 0;
            PublishCountChanged();
        }

        protected override void OnInit()
        {
            if (gameModeConfig == null)
            {
                Debug.LogWarning("[CountStateModule] GameModeConfig가 할당되지 않았습니다.");
            }

            // 이벤트 구독
            Events.Subscribe<MatchStarted>(OnMatchStarted);
            Events.Subscribe<AtBatStartedCommand>(OnAtBatStarted);
            Events.Subscribe<PitchOutcome>(OnPitchOutcome);
            Events.Subscribe<BatterAwardedBase>(OnBatterAwardedBase);
            Events.Subscribe<BatterOut>(OnBatterOut);
            Events.Subscribe<InningHalfStarted>(OnInningHalfStarted);
            Events.Subscribe<InningHalfEnded>(OnInningHalfEnded);
        }

        protected override void OnShutdown()
        {
            // 이벤트 구독 해제
            Events.Unsubscribe<MatchStarted>(OnMatchStarted);
            Events.Unsubscribe<AtBatStartedCommand>(OnAtBatStarted);
            Events.Unsubscribe<PitchOutcome>(OnPitchOutcome);
            Events.Unsubscribe<BatterAwardedBase>(OnBatterAwardedBase);
            Events.Unsubscribe<BatterOut>(OnBatterOut);
            Events.Unsubscribe<InningHalfStarted>(OnInningHalfStarted);
            Events.Unsubscribe<InningHalfEnded>(OnInningHalfEnded);
        }

        #region Event handlers

        private void OnMatchStarted(MatchStarted evt)
        {
            // 경기 시작 시 전체 카운트 리셋
            Balls = 0;
            Strikes = 0;
            Outs = 0;
            PublishCountChanged();
        }

        private void OnAtBatStarted(AtBatStartedCommand evt)
        {
            // 새로운 타석 시작: 볼/스트라이크만 리셋
            Balls = 0;
            Strikes = 0;
            PublishCountChanged();
        }

        private void OnPitchOutcome(PitchOutcome outcome)
        {
            if (gameModeConfig == null)
            {
                Debug.LogWarning("[CountStateModule] GameModeConfig가 없어 PitchOutcome을 처리할 수 없습니다.");
                return;
            }

            bool changed = false;

            switch (outcome.Type)
            {
                case PitchOutcomeType.Ball:
                    changed |= ApplyBall();
                    break;

                case PitchOutcomeType.CalledStrike:
                case PitchOutcomeType.SwingingStrike:
                    changed |= ApplyStrike();
                    break;

                case PitchOutcomeType.Foul:
                    changed |= ApplyFoul();
                    break;

                case PitchOutcomeType.HitByPitch:
                    changed |= ApplyHitByPitch();
                    break;

                case PitchOutcomeType.InPlayFair:
                case PitchOutcomeType.InPlayFoul:
                    // 인플레이/파울 인플레이: 여기서는 카운트를 직접 건드리지 않는다.
                    // 실제 아웃/안타 여부는 타구/주루/수비 모듈에서 BatterOut, BatterAwardedBase 등으로 돌아온다.
                    break;
            }

            if (changed)
                PublishCountChanged();
        }

        private void OnBatterAwardedBase(BatterAwardedBase evt)
        {
            // 타자에게 루상이 부여되었으므로, 타석은 끝났다고 가정하고 카운트를 리셋한다.
            Balls = 0;
            Strikes = 0;
            PublishCountChanged();
        }

        private void OnBatterOut(BatterOut evt)
        {
            // 주의:
            // - 삼진 아웃은 CountStateModule 내부에서 Outs를 이미 증가시키고
            //   BatterOut(OutType=StrikeOut)을 발행했으므로, 여기서 또 처리하면 안 된다.
            // - 필드 플레이(플라이, 땅볼, 라인아웃 등)에 의한 아웃만 여기서 Outs를 증가시키면 된다.

            if (evt.OutType == BatterOutType.StrikeOut)
            {
                // 이미 내부에서 처리된 아웃이므로 무시
                return;
            }

            Outs++;
            // 타석은 끝났다고 보고 볼/스트라이크 리셋
            Balls = 0;
            Strikes = 0;

            PublishCountChanged();
        }

        private void OnInningHalfStarted(InningHalfStarted evt)
        {
            // 새 이닝의 절반 시작: 전체 카운트 리셋
            Balls = 0;
            Strikes = 0;
            Outs = 0;
            PublishCountChanged();
        }

        private void OnInningHalfEnded(InningHalfEnded evt)
        {
            // 절반 이닝 종료 후: 카운트를 0으로 맞춰둔다.
            Balls = 0;
            Strikes = 0;
            // Outs는 다음 InningHalfStarted에서 0으로 초기화된다.
            PublishCountChanged();
        }

        #endregion

        #region Count logic

        private bool ApplyBall()
        {
            Balls++;

            if (gameModeConfig == null)
                return true;

            if (Balls >= gameModeConfig.maxBalls)
            {
                // 볼넷
                // 1루 부여, Walk 플래그 설정
                var award = new BatterAwardedBase
                {
                    BaseCount = 1,
                    IsHit = false,
                    IsWalk = true,
                    IsHitByPitch = false,
                    IsError = false
                };
                Events.Publish(award);

                // 타석 종료
                var atBatEnd = new AtBatEndedCommand
                {
                    ResultType = AtBatResultType.Walk
                };
                Events.Publish(atBatEnd);

                // 카운트 리셋
                Balls = 0;
                Strikes = 0;
            }

            return true;
        }

        private bool ApplyStrike()
        {
            Strikes++;

            if (gameModeConfig == null)
                return true;

            if (Strikes >= gameModeConfig.maxStrikes)
            {
                // 삼진 아웃
                Outs++;

                var batterOut = new BatterOut
                {
                    OutType = BatterOutType.StrikeOut
                };
                Events.Publish(batterOut);

                var atBatEnd = new AtBatEndedCommand
                {
                    ResultType = AtBatResultType.StrikeOut
                };
                Events.Publish(atBatEnd);

                Balls = 0;
                Strikes = 0;
            }

            return true;
        }

        private bool ApplyFoul()
        {
            if (gameModeConfig == null)
            {
                // 기본룰: 스트라이크 2개까지는 증가, 그 이후는 증가 X
                if (Strikes < 2)
                    Strikes++;

                return true;
            }

            // 일반 야구룰: 스트 2개 미만이면 증가, 2 이상이면 유지
            if (Strikes < gameModeConfig.maxStrikes - 1)
            {
                Strikes++;
            }

            return true;
        }

        private bool ApplyHitByPitch()
        {
            // 사구: 1루 부여
            var award = new BatterAwardedBase
            {
                BaseCount = 1,
                IsHit = false,
                IsWalk = false,
                IsHitByPitch = true,
                IsError = false
            };
            Events.Publish(award);

            var atBatEnd = new AtBatEndedCommand
            {
                ResultType = AtBatResultType.HitByPitch
            };
            Events.Publish(atBatEnd);

            Balls = 0;
            Strikes = 0;

            return true;
        }

        #endregion

        private void PublishCountChanged()
        {
            var evt = new CountChanged
            {
                Balls = Balls,
                Strikes = Strikes,
                Outs = Outs
            };
            Events.Publish(evt);
        }
    }
}
