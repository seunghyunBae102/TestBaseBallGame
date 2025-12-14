using Bash.Framework.Core;
using Bash.Framework.Core.Events;
using UnityEngine;

namespace Bash.Core.Ball
{
    [RequireComponent(typeof(BallNode))]
    public class BallPhysicsCompo : ActorCompo
    {
        [Header("Settings")]
        [SerializeField] private LayerMask _collisionMask;
        [SerializeField] private float _bounciness = 0.6f;

        private BallNode _ball;
        private IBallMovementStrategy _currentStrategy;
        private Vector3 _prevPos;

        public bool IsPitchingMode => _currentStrategy is PitchCurveStrategy;

        protected override void OnInit()
        {
            _ball = (BallNode)Owner;
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

        // [Fix] FielderPawn이 호출하는 메서드 추가
        public void StopSimulation()
        {
            _currentStrategy = null;
            if (_ball != null) _ball.Velocity = Vector3.zero;
        }

        // [Fix] FielderPawn이 호출하는 메서드 추가
        public void Launch(Vector3 dir, float speed, IBallMovementStrategy strategy)
        {
            if (_ball != null)
            {
                _ball.Velocity = dir.normalized * speed;
            }
            SetStrategy(strategy);
            ResetSimulation();
        }

        public override void OnTick(float dt)
        {
            if (_currentStrategy == null || _ball == null) return;
            // 속도가 거의 없으면 스킵 (단, 전략이 충돌 체크를 원하면 계속)
            if (_ball.Velocity.sqrMagnitude < 0.01f && _currentStrategy.ShouldCheckCollision()) return;

            Vector3 currentPos = transform.position;
            Vector3 velocity = _ball.Velocity;

            // 전략 이동
            Vector3 nextPos = _currentStrategy.CalculateNextPosition(dt, currentPos, ref velocity);

            // 충돌 처리
            if (_currentStrategy.ShouldCheckCollision())
            {
                Vector3 direction = nextPos - currentPos;
                float distance = direction.magnitude;

                if (distance > 0.001f)
                {
                    if (Physics.SphereCast(currentPos, _ball.Radius, direction.normalized, out RaycastHit hit, distance, _collisionMask))
                    {
                        HandleCollision(hit, ref nextPos, ref velocity);
                    }
                }
            }

            if (!_hasLanded && _currentStrategy is HitPhysicsStrategy && nextPos.y <= 0.05f)
            {
                _hasLanded = true;

                // 이벤트 발행
                Events.Publish(new BallHitGroundEvent
                {
                    LandingPosition = nextPos
                });

                // (선택) 공을 땅에 붙이거나 튀기게 하기
                // 여기서는 일단 바닥을 뚫지 않게 보정
                nextPos.y = 0;
                velocity.y = -velocity.y * 0.5f; // 바닥 튕김 (간단 구현)
            }

            transform.position = nextPos;
            _ball.Velocity = velocity;
            _prevPos = transform.position;
        }

        private void HandleCollision(RaycastHit hit, ref Vector3 nextPos, ref Vector3 velocity)
        {
            Vector3 incoming = velocity;
            Vector3 normal = hit.normal;
            Vector3 reflected = Vector3.Reflect(incoming, normal) * _bounciness;

            velocity = reflected;
            nextPos = hit.point + (normal * _ball.Radius * 1.1f);
        }
    }
}