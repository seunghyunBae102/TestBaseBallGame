//using UnityEngine;

//namespace Bash.Core.Ball
//{
//    // 타격: 중력, 공기저항, 벽 반사
//    public class HitPhysicsStrategy : IBallMovementStrategy
//    {
//        private float _gravity;
//        private float _drag;

//        public HitPhysicsStrategy(float gravity = 15.0f, float drag = 0.3f)
//        {
//            _gravity = gravity;
//            _drag = drag;
//        }

//        public Vector3 CalculateNextPosition(float dt, Vector3 currentPos, ref Vector3 currentVel)
//        {
//            // 1. 물리 연산 (Euler Integration)
//            currentVel.y -= _gravity * dt;       // 중력
//            currentVel -= currentVel * _drag * dt; // 공기 저항

//            return currentPos + (currentVel * dt); // 이동
//        }

//        // 타격 후에는 벽, 땅, 관중석 충돌을 모두 체크해야 함
//        public bool ShouldCheckCollision() => true;
//    }
//}