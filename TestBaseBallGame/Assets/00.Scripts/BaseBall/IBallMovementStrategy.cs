using UnityEngine;

namespace Bash.Core.Ball
{
    public interface IBallMovementStrategy
    {
        /// <summary>
        /// 다음 프레임의 위치와 속도를 계산합니다.
        /// </summary>
        /// <param name="dt">델타 타임</param>
        /// <param name="currentPos">현재 위치</param>
        /// <param name="currentVel">현재 속도 (ref로 갱신)</param>
        /// <returns>계산된 다음 위치</returns>
        Vector3 CalculateNextPosition(float dt, Vector3 currentPos, ref Vector3 currentVel);
    }

    // [기본 전략] : 중력과 공기저항이 있는 등가속도 운동
    public class StandardPhysicsStrategy : IBallMovementStrategy
    {
        private readonly float _gravity;
        private readonly float _drag;

        public StandardPhysicsStrategy(float gravity = 9.81f, float drag = 0.5f)
        {
            _gravity = gravity;
            _drag = drag;
        }

        public Vector3 CalculateNextPosition(float dt, Vector3 currentPos, ref Vector3 currentVel)
        {
            // 1. 중력 적용 (y축)
            currentVel.y -= _gravity * dt;

            // 2. 공기 저항 (속도 반대 방향) - 간단한 선형 저항 모델
            // V = V - (V * drag * dt)
            currentVel -= currentVel * _drag * dt;

            // 3. 위치 이동 (P = P + V * dt)
            return currentPos + (currentVel * dt);
        }
    }
}