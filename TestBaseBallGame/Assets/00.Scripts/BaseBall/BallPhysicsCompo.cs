using UnityEngine;
using Bash.Framework.Core;

namespace Bash.Core.Ball
{
    [RequireComponent(typeof(BallNode))]
    public class BallPhysicsCompo : ActorCompo
    {
        [Header("Settings")]
        [SerializeField] private LayerMask _collisionMask; // 벽, 바닥, 배트 등
        [SerializeField] private float _gravity = 20.0f; // 게임적 허용을 위해 실제보다 강하게
        [SerializeField] private float _drag = 0.2f;
        [SerializeField] private float _bounciness = 0.6f; // 반발 계수
        public bool IsPitchingMode => _currentStrategy is PitchCurveStrategy;
        private BallNode _ball;
        private IBallMovementStrategy _currentStrategy;

        // 디버깅용 궤적 그리기
        private Vector3 _prevPos;

        protected override void OnInit()
        {
            _ball = (BallNode)Owner;
            _currentStrategy = new StandardPhysicsStrategy(_gravity, _drag);
            _prevPos = transform.position;
        }

        public void SetStrategy(IBallMovementStrategy strategy)
        {
            _currentStrategy = strategy;
        }

        public void ResetSimulation()
        {
            _prevPos = transform.position;
        }

        public override void OnTick(float dt)
        {
            if (_currentStrategy == null) return;
            // 속도가 거의 없으면 연산 중지 (Sleep)
            if (_ball.Velocity.sqrMagnitude < 0.01f) return;

            Vector3 currentPos = transform.position;
            Vector3 velocity = _ball.Velocity;

            // 1. 전략을 통해 "가고 싶은 위치" 계산
            Vector3 nextPos = _currentStrategy.CalculateNextPosition(dt, currentPos, ref velocity);

            // 2. CCD (Continuous Collision Detection)
            // 현재 위치와 다음 위치 사이를 레이캐스트로 검사
            Vector3 direction = nextPos - currentPos;
            float distance = direction.magnitude;

            // (이동 거리가 너무 짧으면 검사 패스하되, 최소한의 안전장치는 필요)
            if (distance > 0.001f)
            {
                // Raycast vs SphereCast: 공의 두께를 고려하려면 SphereCast 권장
                if (Physics.SphereCast(currentPos, _ball.Radius, direction.normalized, out RaycastHit hit, distance, _collisionMask))
                {
                    HandleCollision(hit, ref nextPos, ref velocity);
                }
            }

            // 3. 최종 적용
            transform.position = nextPos;
            _ball.Velocity = velocity; // 갱신된 속도(저항/충돌 적용됨) 저장

            // 디버그 시각화
            Debug.DrawLine(_prevPos, transform.position, Color.red, 2.0f);
            _prevPos = transform.position;
        }

        private void HandleCollision(RaycastHit hit, ref Vector3 nextPos, ref Vector3 velocity)
        {
            // A. 반사 벡터 계산 (V_new = Reflect(V_old, Normal) * Bounciness)
            Vector3 incoming = velocity;
            Vector3 normal = hit.normal;
            Vector3 reflected = Vector3.Reflect(incoming, normal) * _bounciness;

            velocity = reflected;

            // B. 위치 보정 (뚫고 들어간 만큼 다시 튕겨 나오게 하기 - 간단 버전)
            // 충돌 지점에서 조금 띄워줌
            nextPos = hit.point + (normal * _ball.Radius * 1.1f);

            // C. 이벤트 발생 (소리, 파티클 등)
            // GameRoot.Instance.Events.Publish(new BallHitEvent(hit)); 
            // 예: 땅에 닿았으면 흙먼지, 벽이면 쿵 소리
        }
    }
}