using UnityEngine;

namespace Bash.Core.Ball
{
    // [인터페이스 정의]
    public interface IBallMovementStrategy
    {
        Vector3 CalculateNextPosition(float dt, Vector3 currentPos, ref Vector3 currentVel);
        bool ShouldCheckCollision();
    }

    // [전략 1: 투구용 커브볼 (베지에)]
    public class PitchCurveStrategy : IBallMovementStrategy
    {
        private Vector3 _p0, _p1, _p2;
        private float _duration, _elapsed;

        public PitchCurveStrategy(Vector3 start, Vector3 end, float curveRight, float curveUp, float speed)
        {
            _p0 = start;
            _p2 = end;
            float dist = Vector3.Distance(start, end);
            _duration = dist / speed;

            Vector3 mid = (_p0 + _p2) * 0.5f;
            _p1 = mid + (Vector3.right * curveRight) + (Vector3.up * curveUp);
        }

        public Vector3 CalculateNextPosition(float dt, Vector3 currentPos, ref Vector3 currentVel)
        {
            _elapsed += dt;
            float t = Mathf.Clamp01(_elapsed / _duration);
            float u = 1 - t;

            Vector3 nextPos = (u * u * _p0) + (2 * u * t * _p1) + (t * t * _p2);
            if (dt > 0) currentVel = (nextPos - currentPos) / dt;

            return nextPos;
        }

        public bool ShouldCheckCollision() => false;
    }

    // [전략 2: 타격/송구용 물리 시뮬레이션]
    public class HitPhysicsStrategy : IBallMovementStrategy
    {
        private float _gravity;
        private float _drag;

        public HitPhysicsStrategy(float gravity = 15.0f, float drag = 0.2f)
        {
            _gravity = gravity;
            _drag = drag;
        }

        public Vector3 CalculateNextPosition(float dt, Vector3 currentPos, ref Vector3 currentVel)
        {
            // Euler 적분
            currentVel.y -= _gravity * dt;
            currentVel -= currentVel * _drag * dt;
            return currentPos + (currentVel * dt);
        }

        public bool ShouldCheckCollision() => true;
    }
}