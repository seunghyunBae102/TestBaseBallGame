using UnityEngine;
using Bash.Framework.Core;

namespace Bash.Core.Unit
{
    public class UnitMovementCompo : ActorCompo
    {
        [Header("Stats")]
        [SerializeField] private float _moveSpeed = 5.0f; // 초속 5m
        [SerializeField] private float _rotationSpeed = 10.0f; // 회전 속도

        private Vector3 _targetPos;
        private bool _isMoving = false;
        private float _arrivalThreshold = 0.1f;

        // 외부(Controller)에서 호출하는 이동 명령
        public void MoveTo(Vector3 target)
        {
            _targetPos = target;
            _targetPos.y = transform.position.y; // 높이는 유지 (평면 이동 가정)
            _isMoving = true;
        }

        public void Stop()
        {
            _isMoving = false;
        }

        public override void OnTick(float dt)
        {
            if (!_isMoving) return;

            Vector3 currentPos = transform.position;
            Vector3 dir = (_targetPos - currentPos);
            float dist = dir.magnitude;

            // 1. 도착 판정
            if (dist <= _arrivalThreshold)
            {
                _isMoving = false;
                transform.position = _targetPos;
                return;
            }

            // 2. 회전 (LookRotation) - 가려는 방향을 바라봄
            dir.Normalize();
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotationSpeed * dt);
            }

            // 3. 이동 (Translate)
            transform.position += dir * _moveSpeed * dt;

            // (추후 여기에 AnimationCompo.SetBool("IsRun", true) 호출)
        }

        // 디버깅용: 목적지 표시
        private void OnDrawGizmos()
        {
            if (_isMoving)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, _targetPos);
                Gizmos.DrawWireSphere(_targetPos, 0.2f);
            }
        }
    }
}