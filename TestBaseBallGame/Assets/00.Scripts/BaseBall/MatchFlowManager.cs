using System.Threading.Tasks;
using UnityEngine;
using Bash.Framework.Core;
using Bash.Framework.Core.Events;
using Bash.Core.Rules;

namespace Bash.Framework.Managers
{
    public enum EMatchState { StandBy, Pitching, InPlay, Resolution }

    public class MatchFlowManager : ActorCompo
    {
        public EMatchState CurrentState { get; private set; }
        private RosterManager _rosterMgr;
        private GameRuleManager _ruleMgr;

        protected override void OnInit()
        {
            _rosterMgr = GameRoot.Instance.GetManager<RosterManager>();
            _ruleMgr = GameRoot.Instance.GetManager<GameRuleManager>();

            // 이벤트 구독 (PitchStartEvent 등)...

            // 게임 시작 (비동기 실행을 위해 별도 메서드 호출)
            StartGameLoop();
        }

        private async void StartGameLoop()
        {
            // 1회초 수비 준비 대기
            if (_rosterMgr != null) await _rosterMgr.PrepareInningAsync(true);
            ChangeState(EMatchState.StandBy);
        }

        private void ChangeState(EMatchState newState)
        {
            CurrentState = newState;
            // UI 이벤트 발행...

            switch (newState)
            {
                case EMatchState.StandBy: ProcessStandBy(); break;
                    // ...
            }
        }

        // --- Async Phase Processing ---

        private async void ProcessStandBy()
        {
            // 타자 준비 대기
            bool isTop = (_ruleMgr.Inning % 2 != 0);
            if (_rosterMgr != null) await _rosterMgr.PrepareNextBatterAsync(isTop);

            // 잠시 연출 대기
            await Task.Delay(1500);
            ChangeState(EMatchState.Pitching);
        }

        // ... (나머지 로직은 이전과 동일, 필요시 await 적용)
    }
}