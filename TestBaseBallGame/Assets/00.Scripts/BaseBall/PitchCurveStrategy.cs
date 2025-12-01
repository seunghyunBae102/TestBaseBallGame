using UnityEngine;

namespace Bash.Core.Ball
{
    // 투구: 시작점 -> 제어점(휘는 정도) -> 끝점(포수)
    public class PitchCurveStrategy : IBallMovementStrategy
    {
        private Vector3 _p0; // 투구 위치 (손)
        private Vector3 _p1; // 제어점 (Control Point - 커브의 휨 결정)
        private Vector3 _p2; // 목표 위치 (포수 미트)

        private float _duration;
        private float _elapsed = 0f;

        public PitchCurveStrategy(Vector3 start, Vector3 end, float curveRight, float curveUp, float speed)
        {
            _p0 = start;
            _p2 = end;

            float dist = Vector3.Distance(start, end);
            _duration = dist / speed; // 도달 시간 계산

            // 제어점(P1) 계산: 시작과 끝의 중간 지점에서, X/Y축으로 오프셋을 줌
            Vector3 mid = (_p0 + _p2) * 0.5f;
            _p1 = mid + (Vector3.right * curveRight) + (Vector3.up * curveUp);
        }

        public Vector3 CalculateNextPosition(float dt, Vector3 currentPos, ref Vector3 currentVel)
        {
            _elapsed += dt;
            float t = Mathf.Clamp01(_elapsed / _duration);

            // [2차 베지에 곡선 공식]
            // B(t) = (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2
            float u = 1 - t;
            Vector3 nextPos = (u * u * _p0) + (2 * u * t * _p1) + (t * t * _p2);

            // 속도 벡터 갱신 (다음 프레임 타격 계산을 위해 현재 이동 방향 저장)
            if (dt > 0) currentVel = (nextPos - currentPos) / dt;

            return nextPos;
        }

        // 투구 중에는 벽 충돌 무시 (배트 충돌은 타자 쪽에서 처리)
        // 만약 '데드볼(사구)'을 구현하려면 여기서 타자 충돌 체크를 할 수도 있음
        public bool ShouldCheckCollision() => false;
    }
}