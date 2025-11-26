// Scripts/Game/Baseball/State/InningStateModule.cs
using UnityEngine;
using Bash.Framework.Core;
using Bash.Framework.Core;
using Bash.Game.Baseball.Config;
using Bash.Game.Baseball.Shared;
using Bash.Game.Baseball.Events;

namespace Bash.Game.Baseball.State
{
    /// <summary>
    /// 이닝 번호, 초/말, 공격/수비 팀 상태를 관리하는 모듈.
    /// - MatchStarted에서 1회초 시작.
    /// - CountChanged에서 아웃 수가 규칙에 도달하면 이닝 절반 종료/교대를 처리.
    /// - InningHalfStarted, InningHalfEnded 이벤트를 발행한다.
    /// </summary>
    public class InningStateModule : Module
    {
        [Header("Config")]
        [SerializeField] private GameModeConfig gameModeConfig;

        public int CurrentInningNumber { get; private set; } = 1;
        public bool IsTop { get; private set; } = true; // true = 초, false = 말
        public TeamSide OffenseSide { get; private set; } = TeamSide.Away;
        public TeamSide DefenseSide { get; private set; } = TeamSide.Home;

        private bool _matchActive;

        protected override void OnInit()
        {
            if (gameModeConfig == null)
            {
                Debug.LogWarning("[InningStateModule] GameModeConfig가 할당되지 않았습니다.");
            }

            Events.Subscribe<MatchStarted>(OnMatchStarted);
            Events.Subscribe<CountChanged>(OnCountChanged);
            Events.Subscribe<MatchEnded>(OnMatchEnded);
        }

        protected override void OnShutdown()
        {
            Events.Unsubscribe<MatchStarted>(OnMatchStarted);
            Events.Unsubscribe<CountChanged>(OnCountChanged);
            Events.Unsubscribe<MatchEnded>(OnMatchEnded);
        }

        #region Event handlers

        private void OnMatchStarted(MatchStarted evt)
        {
            _matchActive = true;

            CurrentInningNumber = 1;
            IsTop = true;

            // 기본 규칙: 원정팀 공격, 홈팀 수비로 시작
            OffenseSide = TeamSide.Away;
            DefenseSide = TeamSide.Home;

            PublishInningHalfStarted();
        }

        private void OnMatchEnded(MatchEnded evt)
        {
            _matchActive = false;
        }

        private void OnCountChanged(CountChanged evt)
        {
            if (!_matchActive) return;
            if (gameModeConfig == null) return;

            if (evt.Outs < gameModeConfig.outsPerHalfInning)
                return;

            // 아웃이 규칙에 도달했으므로, 현재 이닝 절반을 종료한다.
            PublishInningHalfEnded();

            // 다음 이닝 상태로 이동
            if (IsTop)
            {
                // 초에서 말로
                IsTop = false;

                OffenseSide = TeamSide.Home;
                DefenseSide = TeamSide.Away;
            }
            else
            {
                // 말에서 다음 이닝 초로
                IsTop = true;
                CurrentInningNumber++;

                OffenseSide = TeamSide.Away;
                DefenseSide = TeamSide.Home;
            }

            // 새 이닝 절반 시작 알림
            PublishInningHalfStarted();

            // 실제로 경기를 종료할지(콜드게임, 최대 이닝, 무승부 등)는
            // 별도의 MatchRuleModule 등에서
            // GameModeConfig + ScoreStateModule + 이 모듈 상태를 보고 판단.
        }

        #endregion

        #region Helper: publish events

        private void PublishInningHalfStarted()
        {
            var evt = new InningHalfStarted
            {
                InningNumber = CurrentInningNumber,
                IsTop = IsTop,
                Offense = OffenseSide,
                Defense = DefenseSide
            };

            Events.Publish(evt);
        }

        private void PublishInningHalfEnded()
        {
            var evt = new InningHalfEnded
            {
                InningNumber = CurrentInningNumber,
                IsTop = IsTop,
                Offense = OffenseSide,
                Defense = DefenseSide
            };

            Events.Publish(evt);
        }

        #endregion
    }
}
