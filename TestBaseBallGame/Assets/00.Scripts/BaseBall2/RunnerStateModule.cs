// Scripts/Game/Baseball/Runner/RunnerStateModule.cs
using UnityEngine;
using Bash.Framework.Core;
using Bash.Game.Baseball.Shared;
using Bash.Game.Baseball.Events;

namespace Bash.Game.Baseball.Runner
{
    /// <summary>
    /// 현재 공격 팀의 베이스 점유 상태를 관리하는 모듈.
    /// - 베이스별 RunnerId를 유일하게 저장
    /// - RunnerAdvanceRequest / RunnerOutEvent / InningHalfStarted / AtBatStarted 처리
    /// </summary>
    public class RunnerStateModule : Module
    {
        private struct RunnerSlot
        {
            public bool occupied;
            public RunnerId runner;
        }

        private RunnerSlot[] _bases = new RunnerSlot[4]; // 0~3
        private TeamSide _offenseTeam;

        protected override void OnInit()
        {
            var events = GameRoot.Instance.Events;
            events.Subscribe<InningHalfStarted>(OnInningHalfStarted);
            events.Subscribe<AtBatStarted>(OnAtBatStarted);
            events.Subscribe<AtBatEnded>(OnAtBatEnded);
            events.Subscribe<RunnerAdvanceRequest>(OnRunnerAdvanceRequest);
            events.Subscribe<RunnerOut>(OnRunnerOutEvent);
        }

        protected override void OnShutdown()
        {
            var events = GameRoot.Instance.Events;
            events.Unsubscribe<InningHalfStarted>(OnInningHalfStarted);
            events.Unsubscribe<AtBatStarted>(OnAtBatStarted);
            events.Unsubscribe<AtBatEnded>(OnAtBatEnded);
            events.Unsubscribe<RunnerAdvanceRequest>(OnRunnerAdvanceRequest);
            events.Unsubscribe<RunnerOut>(OnRunnerOutEvent);
        }

        private void ResetBases()
        {
            for (int i = 0; i < _bases.Length; i++)
            {
                _bases[i].occupied = false;
                _bases[i].runner = default;
            }
        }

        private void OnInningHalfStarted(InningHalfStarted e)
        {
            _offenseTeam = e.Offense;
            ResetBases();
        }

        private void OnAtBatStarted(AtBatStarted e)
        {
            // 공격 팀이 바뀌었으면 통일
            _offenseTeam = e.OffenseTeam;
            // 홈(타석)에 타자 배치
            SetRunnerOnBase(BaseIndex.Home, e.Batter);
        }

        private void OnAtBatEnded(AtBatEnded e)
        {
            // 타석 종료 시, 홈 베이스(타석) 비우기
            ClearBase(BaseIndex.Home);
        }

        private void OnRunnerOutEvent(RunnerOut e)
        {
            // 해당 베이스에 해당 러너가 있다면 제거
            int idx = (int)e.AtBase;
            if (idx < 0 || idx >= _bases.Length) return;

            if (_bases[idx].occupied && RunnerUtil.Equals(_bases[idx].runner, e.Runner))
            {
                _bases[idx].occupied = false;
            }
        }

        private void OnRunnerAdvanceRequest(RunnerAdvanceRequest req)
        {
            // FromBase 기준으로 이동 시도
            int from = (int)req.FromBase;
            if (from < 0 || from >= _bases.Length)
                return;

            // 해당 베이스에 실제로 그 러너가 있는지 확인
            if (!_bases[from].occupied || !RunnerUtil.Equals(_bases[from].runner, req.Runner))
            {
                // 혹시 fromBase가 틀렸을 수도 있으니, 전체 검색해서 찾아본다.
                int foundIndex = FindRunnerIndex(req.Runner);
                if (foundIndex < 0)
                {
                    // 존재하지 않는 runner 요청이면 무시
                    return;
                }
                from = foundIndex;
            }

            int targetIndex = from + req.AdvanceCount;

            // 홈(0)에서 +1 → 1루, +2 → 2루 ...  
            // 3루 이상 넘어가면 득점
            if (targetIndex > (int)BaseIndex.Third)
            {
                // 득점 처리
                EmitRunnerScored(req.Runner);
                // 원래 있던 베이스 비우기
                _bases[from].occupied = false;
                return;
            }

            // 베이스 이동
            int ti = Mathf.Clamp(targetIndex, 0, 3);

            if (_bases[ti].occupied)
            {
                if (req.Forced)
                {
                    // 기존 러너 포스 아웃
                    var forcedOut = new RunnerOut
                    {
                        Runner = _bases[ti].runner,
                        AtBase = (BaseIndex)ti,
                        Reason = OutReason.ForceOut
                    };
                    GameRoot.Instance.Events.Publish(forcedOut);

                    _bases[ti].occupied = false;
                }
                else
                {
                    // 비강제인데 이미 누가 있다면 이 룰 모듈이 잘못 구성된 것.
                    Debug.LogWarning($"[RunnerState] 비강제 이동 목표 베이스({(BaseIndex)ti})에 이미 러너가 있습니다.");
                    return;
                }
            }

            // 실제 이동
            var runner = _bases[from].runner;
            _bases[from].occupied = false;
            _bases[ti].occupied = true;
            _bases[ti].runner = runner;
        }

        private void EmitRunnerScored(RunnerId runner)
        {
            var e = new RunnerScored    
            {
                Runner = runner,
                Runs = 1
            };
            GameRoot.Instance.Events.Publish(e);
        }

        private int FindRunnerIndex(RunnerId runner)
        {
            for (int i = 0; i < _bases.Length; i++)
            {
                if (_bases[i].occupied && RunnerUtil.Equals(_bases[i].runner, runner))
                    return i;
            }
            return -1;
        }

        private void SetRunnerOnBase(BaseIndex baseIndex, RunnerId runner)
        {
            int idx = (int)baseIndex;
            if (idx < 0 || idx >= _bases.Length) return;

            _bases[idx].occupied = true;
            _bases[idx].runner = runner;
        }

        private void ClearBase(BaseIndex baseIndex)
        {
            int idx = (int)baseIndex;
            if (idx < 0 || idx >= _bases.Length) return;
            _bases[idx].occupied = false;
        }

        // --- 외부에서 상태 조회용 간단 API ---

        public bool TryGetRunnerOnBase(BaseIndex baseIndex, out RunnerId runner)
        {
            int idx = (int)baseIndex;
            if (idx < 0 || idx >= _bases.Length || !_bases[idx].occupied)
            {
                runner = default;
                return false;
            }

            runner = _bases[idx].runner;
            return true;
        }
    }
}
