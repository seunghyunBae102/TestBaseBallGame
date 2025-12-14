using UnityEngine;
using Bash.Framework.Core;
using Bash.Core.Unit;
using Bash.Framework.Core.Events;
using Bash.Framework.Managers;
using Bash.Core.Ball; // 공 위치 파악용

namespace Bash.Core.GamePlay
{
    public class RunnerController : UnitController
    {
        private RunnerPawn Runner => ControlledPawn as RunnerPawn;

        // 상태 확장
        private enum ERunnerState { Idle, Leading, Stealing, Running, Returning, Approaching }
        private ERunnerState _state = ERunnerState.Idle;

        [Header("AI Settings")]
        [SerializeField] private float _stealAggressiveness = 0.3f; // 도루 적극성 (0~1)

        protected override void OnInit()
        {
            base.OnInit();
            Events.Subscribe<RoundStartEvent>(OnRoundStart);
            Events.Subscribe<PitchStartEvent>(OnPitchStart);
            Events.Subscribe<BallHitEvent>(OnBallHit);
            Events.Subscribe<BallCaughtEvent>(OnBallCaught);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Events.Unsubscribe<RoundStartEvent>(OnRoundStart);
            Events.Unsubscribe<PitchStartEvent>(OnPitchStart);
            Events.Unsubscribe<BallHitEvent>(OnBallHit);
            Events.Unsubscribe<BallCaughtEvent>(OnBallCaught);
        }

        // 1. 라운드 시작 -> 리드 잡기
        private void OnRoundStart(RoundStartEvent evt)
        {
            if (!HasPawn || Runner.CurrentBase == null) return;

            // [스탯 적용] 속도와 배짱(StressResist)이 좋을수록 리드를 길게 잡음
            float speed = Runner.Stat.GetStat(EStatType.Speed);
            float bravery = Runner.Stat.GetStat(EStatType.StressResist);

            // 기본 2m + 스탯 보너스 (최대 4~5m)
            float leadDist = 2.0f + (speed * 0.02f) + (bravery * 0.01f);

            _state = ERunnerState.Leading;
            Runner.TakeLead(leadDist);
            Debug.Log($"[RunnerAI] Taking lead: {leadDist:F2}m");
        }

        // 2. 투구 시작 -> 도루 판단
        private void OnPitchStart(PitchStartEvent evt)
        {
            if (!HasPawn || _state != ERunnerState.Leading) return;

            // 도루 시도 여부 결정
            // (실제로는 작전 지시가 우선이지만, 여기선 독자 판단)
            float speed = Runner.Stat.GetStat(EStatType.Speed);

            // 속도가 120 이상이고 난수가 적극성보다 낮으면 도루
            bool trySteal = (speed >= 120f) && (Random.value < _stealAggressiveness);

            if (trySteal)
            {
                Debug.Log("<color=yellow>[RunnerAI] STEAL ATTEMPT!</color>");
                _state = ERunnerState.Stealing;
                Runner.RunToNextBase();
            }
        }

        // 3. 타격 발생 -> (기존 로직 + 도루 중이었다면 계속 달림)
        private void OnBallHit(BallHitEvent evt)
        {
            if (!HasPawn) return;

            // 도루 중이었다면 타격음 무시하고 계속 달림 (히트 앤 런)
            if (_state == ERunnerState.Stealing)
            {
                _state = ERunnerState.Running;
                return;
            }

            // (기존 타격 반응 로직 - Perception Delay 등...)
            // 편의상 즉시 출발로 간략화
            _state = ERunnerState.Running;
            Runner.RunToNextBase();
        }

        public override void OnTick(float dt)
        {
            // 4. 추가 진루(Overrun) 판단 (Running 상태일 때만)
            if (_state == ERunnerState.Running)
            {
                CheckExtraBaseOpportunity();
            }
        }

        private void CheckExtraBaseOpportunity()
        {
            // 베이스에 가까워졌을 때 판단
            if (Runner.IsApproachingBase())
            {
                // 공 위치 파악
                var ball = GameRoot.Instance.FindNode<BallNode>();
                if (ball == null) return;

                // 다음 베이스(목표+1)까지의 거리 계산 (예: 1루->2루 뛰는데 3루까지 갈지)
                // 현재 구조상 RunnerPawn은 TargetBase만 알고 있음.
                // 여기서는 "공이 외야(깊은 곳)에 있는가?"로 단순 판단.

                float ballDistFromHome = Vector3.Distance(Vector3.zero, ball.transform.position);

                // 공이 60m 이상 멀리 있고(외야), 아직 누군가 잡지 못했다면(속도가 있음)
                if (ballDistFromHome > 60.0f && ball.Velocity.magnitude > 1.0f)
                {
                    Debug.Log("[RunnerAI] Extra Base Decision: GO FOR IT!");
                    // 상태 변경 없이, RunnerPawn에게 "도착 후 멈추지 말고 다음 베이스로 타겟 변경해"라고 명령해야 함.
                    // 현재 RunnerPawn.RunToNextBase()는 "현재 베이스 기준 다음"만 찾음.
                    // 따라서, OnTriggerEnter에서 멈추지 않게 하는 플래그나, 강제로 타겟을 갱신하는 로직 필요.

                    // [임시 해결] Pawn 레벨에서 자동 정지를 끄고, 목표를 갱신
                    // (이 부분은 RunnerPawn 구조 개선이 필요하지만, AI 로직상 이렇게 판단함)
                }
            }
        }

        // ... OnBallCaught 등 기존 로직 유지 ...
    }
}