// Scripts/Game/Baseball/State/ScoreStateModule.cs
using UnityEngine;
using Bash.Framework.Core;
using Bash.Framework.Core;
using Bash.Game.Baseball.Shared;
using Bash.Game.Baseball.Events;

namespace Bash.Game.Baseball.State
{
    /// <summary>
    /// 홈/원정 점수를 관리하는 상태 모듈.
    /// - MatchStarted에서 점수 리셋.
    /// - RunsScored 이벤트를 받아 점수를 누적.
    /// - MatchEnded 이후에도 상태는 조회 가능(리플레이/요약 등).
    /// </summary>
    public class ScoreStateModule : Module
    {
        public int HomeScore { get; private set; }
        public int AwayScore { get; private set; }

        protected override void OnInit()
        {
            Events.Subscribe<MatchStarted>(OnMatchStarted);
            Events.Subscribe<RunsScored>(OnRunsScored);
            Events.Subscribe<MatchEnded>(OnMatchEnded);
        }

        protected override void OnShutdown()
        {
            Events.Unsubscribe<MatchStarted>(OnMatchStarted);
            Events.Unsubscribe<RunsScored>(OnRunsScored);
            Events.Unsubscribe<MatchEnded>(OnMatchEnded);
        }

        #region Event handlers

        private void OnMatchStarted(MatchStarted evt)
        {
            HomeScore = 0;
            AwayScore = 0;
        }

        private void OnRunsScored(RunsScored evt)
        {
            if (evt.RunCount <= 0)
                return;

            if (evt.Team == TeamSide.Home)
            {
                HomeScore += evt.RunCount;
            }
            else
            {
                AwayScore += evt.RunCount;
            }

            // 필요하다면 여기서 ScoreChanged 이벤트를 추가로 발행할 수 있음.
            // (2단계에 ScoreChanged 타입을 추가한 뒤 사용)
        }

        private void OnMatchEnded(MatchEnded evt)
        {
            // 여기서는 상태만 유지한다.
            // MatchEnded 이벤트는 상위 룰/매니저에서 발행한다.
            // ScoreStateModule는 최종 스코어를 제공하는 역할만.
        }

        #endregion

        #region Helper

        /// <summary>
        /// 특정 팀의 점수를 반환한다.
        /// </summary>
        public int GetScore(TeamSide side)
        {
            return side == TeamSide.Home ? HomeScore : AwayScore;
        }

        #endregion
    }
}
