// Scripts/Game/Baseball/Match/Modules/Rule_RunnerAdvanceModule.cs
using UnityEngine;


    /// <summary>
    /// 주자 베이스 상태를 관리하고,
    /// BatterAwardedBase 이벤트(볼넷/HBP/안타/홈런)를 받아
    /// 강제 진루 및 RunsScored를 계산하는 모듈.
    /// 
    /// *단순 버전*: 모든 진루는 "강제"라고 가정 (선택 진루/아웃 없음).
    /// 나중에 주루AI/수비플레이를 추가하면서 확장할 수 있다.
    /// </summary>
    public class Rule_RunnerAdvanceModule : Module
    {
        // 현재 공격팀 – 이닝 상태 모듈에서 받아서 업데이트해도 됨
        private TeamSide _currentOffense = TeamSide.Away;

        // 베이스 점유 상태
        private bool _onFirst;
        private bool _onSecond;
        private bool _onThird;

        protected override void OnInit()
        {
            base.OnInit();
            Events.Subscribe<BatterAwardedBase>(OnBatterAwardedBase);
            Events.Subscribe<InningHalfStarted>(OnInningHalfStarted);
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            Events.Unsubscribe<BatterAwardedBase>(OnBatterAwardedBase);
            Events.Unsubscribe<InningHalfStarted>(OnInningHalfStarted);
        }

        private void OnInningHalfStarted(InningHalfStarted e)
        {
            // 이닝이 바뀌면 주자 초기화
            _currentOffense = e.Offense;
            _onFirst = _onSecond = _onThird = false;
        }

        private void OnBatterAwardedBase(BatterAwardedBase e)
        {
            int bases = Mathf.Max(1, e.Bases);
            int runs = 0;

            // 간단히, 홈에서 역순으로 처리 (3루 → 홈, 2루 → 3루, 1루 → 2루, 타자 → 1루)
            // 모든 진루는 강제라고 보고, 넘어가는 베이스는 전부 득점 처리.

            // 3루 주자부터
            if (_onThird)
            {
                int dest = 3 + bases;
                if (dest >= 4) runs++;       // 홈 도착
                else if (dest == 3) _onThird = true;   // 원래 위치 유지 (이론상 안 나옴)
                else if (dest == 2) _onSecond = true;  // 후진은 없다고 가정
                _onThird = false; // 일단 이동 처리했으니 비움
            }

            // 2루
            if (_onSecond)
            {
                int dest = 2 + bases;
                if (dest >= 4) runs++;
                else if (dest == 3) _onThird = true;
                else if (dest == 2) _onSecond = true;
                _onSecond = false;
            }

            // 1루
            if (_onFirst)
            {
                int dest = 1 + bases;
                if (dest >= 4) runs++;
                else if (dest == 3) _onThird = true;
                else if (dest == 2) _onSecond = true;
                _onFirst = false;
            }

            // 타자
            int batterDest = bases;
            if (batterDest >= 4) runs++;
            else if (batterDest == 3) _onThird = true;
            else if (batterDest == 2) _onSecond = true;
            else if (batterDest == 1) _onFirst = true;

            if (runs > 0)
            {
                Events.Publish(new RunsScored
                {
                    Team = _currentOffense,
                    RunCount = runs
                });
            }
        }
    }


