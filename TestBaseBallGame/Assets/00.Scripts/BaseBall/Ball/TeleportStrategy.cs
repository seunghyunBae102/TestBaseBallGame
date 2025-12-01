using UnityEngine;
using Bash.Core.Ball;

public class TeleportStrategy : IBallMovementStrategy
{
    private Vector3 _start;
    private Vector3 _end;
    private float _duration;
    private float _elapsed = 0f;

    // 사라지는 구간 (0.0 ~ 1.0)
    private float _vanishStart = 0.3f; // 30% 지점에서 사라짐
    private float _vanishEnd = 0.8f;   // 80% 지점에서 나타남

    public TeleportStrategy(Vector3 start, Vector3 end, float speed)
    {
        _start = start;
        _end = end;
        _duration = Vector3.Distance(start, end) / speed;
    }

    public Vector3 CalculateNextPosition(float dt, Vector3 currentPos, ref Vector3 currentVel)
    {
        _elapsed += dt;
        float t = Mathf.Clamp01(_elapsed / _duration);

        // 1. 순간이동 구간 체크
        if (t > _vanishStart && t < _vanishEnd)
        {
            // 땅속이나 아주 먼 곳으로 보내서 안 보이게 처리 (Renderer 끄는 것보다 물리적으로 안전)
            // 혹은 그냥 직전 위치를 유지하며 멈춰있는 것처럼 보이게 할 수도 있음
            return new Vector3(0, -1000, 0);
        }

        // 2. 선형 보간 (Lerp) 이동
        Vector3 nextPos = Vector3.Lerp(_start, _end, t);

        // 속도 계산 (타격 시 필요)
        if (dt > 0) currentVel = (_end - _start) / _duration;

        return nextPos;
    }

    public bool ShouldCheckCollision() => false;
}