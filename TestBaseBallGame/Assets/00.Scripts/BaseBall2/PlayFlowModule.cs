// Scripts/Game/Baseball/Playflow/PlayFlowModule.cs
using UnityEngine;
using Bash.Framework.Core;
using Bash.Game.Baseball.Events;
using Bash.Game.Baseball.Shared;
using Bash.Framework.Node;

namespace Bash.Game.Baseball.Playflow
{
    /// <summary>
    /// 경기 전체의 상위 흐름(타석 진행, 이닝 전환)을 오케스트레이션하는 모듈.
    /// - 직접 룰을 계산하기보다는 이벤트들을 시퀀싱한다.
    /// </summary>
    public class PlayFlowModule : Module
    {
        private enum PlayPhase
        {
            WaitingForPitch,
            PitchInFlight,
            BallInPlay,
            BetweenPlays
        }

        [Header("Lineup")]
        [Tooltip("홈 팀 타순 인원 수 (간단 버전).")]
        [SerializeField] private int homeBattingOrderSize = 9;

        [Tooltip("원정 팀 타순 인원 수 (간단 버전).")]
        [SerializeField] private int awayBattingOrderSize = 9;

        private TeamSide _offenseTeam;
        private PlayPhase _phase;

        private int _homeNextBatterIndex;
        private int _awayNextBatterIndex;

        protected override void OnInit()
        {
            _phase = PlayPhase.BetweenPlays;
            _homeNextBatterIndex = 0;
            _awayNextBatterIndex = 0;

            Events.Subscribe<MatchStarted>(OnMatchStarted);
            Events.Subscribe<InningHalfStarted>(OnInningHalfStarted);
            Events.Subscribe<AtBatEnded>(OnAtBatEnded);
            Events.Subscribe<PitchOutcome>(OnPitchOutcome);
            Events.Subscribe<HitBallResult>(OnHitBallResult);
        }

        protected override void OnShutdown()
        {
            Events.Unsubscribe<MatchStarted>(OnMatchStarted);
            Events.Unsubscribe<InningHalfStarted>(OnInningHalfStarted);
            Events.Unsubscribe<AtBatEnded>(OnAtBatEnded);
            Events.Unsubscribe<PitchOutcome>(OnPitchOutcome);
            Events.Unsubscribe<HitBallResult>(OnHitBallResult);
        }

        private void OnMatchStarted(MatchStarted e)
        {
            // 첫 이닝의 공격 팀은 InningHalfStarted에서 알려준다고 가정
            _phase = PlayPhase.BetweenPlays;
        }

        private void OnInningHalfStarted(InningHalfStarted e)
        {
            _offenseTeam = e.Offense;
            _phase = PlayPhase.BetweenPlays;

            StartNextAtBat();
        }

        private void OnAtBatEnded(AtBatEnded e)
        {
            if (e.OffenseTeam != _offenseTeam)
                return;

            _phase = PlayPhase.BetweenPlays;

            // 여기서 아웃카운트가 3아웃인지 여부는 CountStateModule이 관리한다고 가정.
            // 3아웃이 되면 InningHalfEnded 이벤트가 따로 발행될 것이고,
            // 그걸 InningStateModule/PlayFlow가 처리한다.
            StartNextAtBat();
        }

        private void OnPitchOutcome(PitchOutcome e)
        {
            // 단순히 Phase만 업데이트
            switch (e.Type)
            {
                case PitchOutcomeType.InPlayFair:
                    _phase = PlayPhase.BallInPlay;
                    break;
                case PitchOutcomeType.Ball:
                case PitchOutcomeType.CalledStrike:
                case PitchOutcomeType.SwingingStrike:
                case PitchOutcomeType.Foul:
                case PitchOutcomeType.HitByPitch:
                    // 카운트/타석 종료 여부는 CountStateModule/RunnerRule/PlayFlow 조합으로 처리
                    break;
            }
        }

        private void OnHitBallResult(HitBallResult result)
        {
            if (!result.IsFair)
                return;

            // 인플레이 타구가 끝났으므로 BetweenPlays로 전환
            _phase = PlayPhase.BetweenPlays;

            // 여기서는 별도의 처리 없이 AtBatEnded 이벤트를 기다린다.
            // (RunnerRule/CountState 쪽에서 타석 종료 여부를 판단해서 AtBatEnded를 발행한다고 가정)
        }

        /// <summary>
        /// 현재 공격 팀의 다음 타자를 계산하여 AtBatStarted 이벤트를 발행한다.
        /// </summary>
        private void StartNextAtBat()
        {
            var batterIndex = GetAndAdvanceBatterIndex(_offenseTeam);

            // 실제 ActorNode(캐릭터)는 아직 모르므로 null로 두고,
            // 나중에 Character/Lineup 시스템에서 바인딩하는 구조로 둔다.
            var runnerId = new RunnerId
            {
                batterIndex = batterIndex,
                team = _offenseTeam,
                actor = null
            };

            var atBatStarted = new AtBatStarted
            {
                OffenseTeam = _offenseTeam,
                Batter = runnerId,
                BattingOrderIndex = batterIndex
            };

            Events.Publish(atBatStarted);

            _phase = PlayPhase.WaitingForPitch;
        }

        private int GetAndAdvanceBatterIndex(TeamSide team)
        {
            if (team == TeamSide.Home)
            {
                int index = _homeNextBatterIndex;
                _homeNextBatterIndex = (_homeNextBatterIndex + 1) % Mathf.Max(1, homeBattingOrderSize);
                return index;
            }
            else
            {
                int index = _awayNextBatterIndex;
                _awayNextBatterIndex = (_awayNextBatterIndex + 1) % Mathf.Max(1, awayBattingOrderSize);
                return index;
            }
        }
    }
}
