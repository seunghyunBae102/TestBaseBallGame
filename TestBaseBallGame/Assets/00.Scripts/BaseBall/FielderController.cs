using Bash.Core.Ball;
using Bash.Core.Data; // EFieldPosition
using Bash.Core.GamePlay.AI;
using Bash.Core.GamePlay.Environment;
using Bash.Core.Unit;
using Bash.Framework.Core;
using Bash.Framework.Managers;
using UnityEngine;

namespace Bash.Core.GamePlay
{
    public class FielderController : UnitController
    {
        // 현재 상태 정의
        public enum EAIState { Idle, Chasing, Covering, Holding }

        [Header("Identity")]
        public EFieldPosition PositionID = EFieldPosition.None; // 내 포지션

        [Header("AI Config")]
        [SerializeField] private float _catchRadius = 1.2f;

        private FielderPawn Fielder => ControlledPawn as FielderPawn;
        private EAIState _currentState = EAIState.Idle;
        private BallNode _targetBall;
        private BaseNode _targetBase; // 커버 들어갈 베이스

        // [변경 1] 기존 OnInit에서 이벤트 구독 로직 삭제!
        // 스스로 판단하지 않고 Director의 명령을 기다림.
        protected override void OnInit()
        {
            base.OnInit();
            // Events.Subscribe<BallHitEvent>(OnBallHit); <-- 삭제됨
        }

        // [변경 2] 명령 수신 API 추가

        /// <summary>
        /// 명령 1: 공을 쫓아라 (Chaser)
        /// </summary>
        public void OrderChase(BallNode ball)
        {
            _targetBall = ball;
            _currentState = EAIState.Chasing;
            // Debug.Log($"[{PositionID}] Ordered to Chase!");
        }

        /// <summary>
        /// 명령 2: 베이스를 커버해라 (Cover)
        /// </summary>
        public void OrderCover(BaseNode baseNode)
        {
            _targetBase = baseNode;
            _targetBall = null;
            _currentState = EAIState.Covering;

            // 해당 베이스의 '수비 위치'로 이동
            if (Fielder && Fielder.Movement)
            {
                Fielder.Movement.MoveTo(baseNode.GetCatchPosition());
                Fielder.LookAt(Vector3.zero); // 시선은 필드 안쪽
            }
        }

        /// <summary>
        /// 명령 3: 대기하라 (Idle)
        /// </summary>
        public void OrderIdle()
        {
            _currentState = EAIState.Idle;
            if (Fielder && Fielder.Movement) Fielder.Movement.Stop();
        }

        public override void OnTick(float dt)
        {
            if (!HasPawn) return;

            switch (_currentState)
            {
                case EAIState.Idle:
                    break;

                case EAIState.Chasing:
                    ProcessChasing(); // 기존 로직 재활용
                    break;

                case EAIState.Covering:
                    // 베이스로 이동 중... (MoveTo가 알아서 함)
                    // 도착 후 포구 준비 애니메이션 등이 필요하다면 여기에 추가
                    break;

                case EAIState.Holding:
                    // 송구 대기
                    break;
            }
        }

        private void ProcessChasing()
        {
            if (_targetBall == null) { _currentState = EAIState.Idle; return; }

            Vector3 ballPos = _targetBall.transform.position;
            Vector3 chasePos = new Vector3(ballPos.x, Fielder.transform.position.y, ballPos.z);

            Fielder.Movement.MoveTo(chasePos);
            Fielder.LookAt(ballPos);

            float dist = Vector3.Distance(Fielder.transform.position, ballPos);
            if (dist <= _catchRadius && ballPos.y <= 2.0f)
            {
                PerformCatch();
            }
        }

        private void PerformCatch()
        {
            Fielder.CatchBall(_targetBall);
            _currentState = EAIState.Holding;

            // [임시] 잡으면 무조건 1루 송구 (나중에 이것도 Director가 시키거나 OutLogic 사용)
            var timer = GameRoot.Instance.GetManager<Bash.Framework.Managers.TimerManager>();
            timer.SetTimer(1.0f, DecideAndThrow);
        }

        private void DecideAndThrow()
        {
            // 1. 판단 시스템 가져오기
            var aiSystem = GameRoot.Instance.GetManager<ThrowingDecisionSystem>();
            var fieldMgr = GameRoot.Instance.GetManager<FieldManager>();

            if (aiSystem != null && fieldMgr != null)
            {
                // 2. 내 투척 능력치 가져오기 (대략적 속도)
                // 힘 + 투척력 평균을 35m/s 정도로 환산
                float power = 35.0f;
                if (Fielder.Stat)
                {
                    float str = Fielder.Stat.GetStat(EStatType.Strength);
                    float thr = Fielder.Stat.GetStat(EStatType.Throwing);
                    power = 25.0f + ((str + thr) * 0.1f);
                }

                // 3. AI에게 물어보기
                BaseNode bestBase = aiSystem.GetBestThrowTarget(Fielder.transform.position, power);

                // 4. 결과가 없으면(다 세이프일듯) 1루로 던짐 (안전빵)
                if (bestBase == null)
                {
                    Debug.Log("[AI] No good target. Defaulting to 1st.");
                    bestBase = fieldMgr.CurrentField.GetBase(BaseNode.EBaseType.First);
                }
                else
                {
                    Debug.Log($"[AI] Smart Throw to {bestBase.BaseType}!");
                }

                // 5. 실행
                Fielder.ThrowBall(bestBase);

                _currentState = EAIState.Idle;
                _targetBall = null;
            }
        }
    }
}