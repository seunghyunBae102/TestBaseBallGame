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


        private UnitPawn _pawn;

        protected override void OnInit()
        {
            base.OnInit();
            _pawn = Owner as UnitPawn; // 스탯 접근을 위해 Pawn 캐싱
            // InitializeSteering(); // (기존 Steering 초기화 함수)
        }

        public override void OnTick(float dt)
        {
            if (!_isMoving) return;

            // [스탯 적용 1] 속도 계산 (Speed 스탯)
            // 기본 4m/s + (스탯 * 0.05) -> 스탯 100일 때 9m/s
            float statSpeed = (_pawn && _pawn.Stat) ? _pawn.Stat.GetStat(EStatType.Speed) : 50f;
            float currentMoveSpeed = 4.0f + (statSpeed * 0.05f);

            // [스탯 적용 2] 회전력 계산 (Reflex 스탯)
            // 빠릿빠릿하게 도는지, 둔하게 도는지
            float statReflex = (_pawn && _pawn.Stat) ? _pawn.Stat.GetStat(EStatType.Reflex) : 50f;
            float currentRotSpeed = 5.0f + (statReflex * 0.2f);

            // [스탯 적용 3] 스태미너 페널티 (Stamina)
            if (_pawn && _pawn.Stat)
            {
                if (_pawn.Stat.CurrentStamina <= 1.0f)
                {
                    currentMoveSpeed *= 0.5f; // 지치면 속도 반토막
                }
                else
                {
                    _pawn.Stat.ConsumeStamina(5.0f * dt); // 뛰면 스태미너 소모
                }
            }

            // --- 이동 로직 ---
            float dist = Vector3.Distance(transform.position, _targetPos);
            if (dist <= _arrivalThreshold)
            {
                _isMoving = false;
                transform.position = _targetPos;
                return;
            }

            // Context Steering (BestDir 계산은 생략 - 이전 코드 참조)
            Vector3 bestDir = (_targetPos - transform.position).normalized; // 임시: 직선 이동

            if (bestDir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(bestDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, currentRotSpeed * dt);
                transform.position += bestDir * currentMoveSpeed * dt;
            }
        }
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