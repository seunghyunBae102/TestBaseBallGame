using UnityEngine;
using Bash.Framework.Core;
using Bash.Core.Unit;
using Bash.Core.GamePlay.Environment; // BaseNode
using Bash.Framework.Managers; // FieldManager

namespace Bash.Core.GamePlay
{
    public class RunnerPawn : UnitPawn
    {
        [Header("Runner State")]
        [SerializeField] private BaseNode _currentBase; // 현재 점유 중인 베이스
        [SerializeField] private BaseNode _targetBase;  // 달리고 있는 목표 베이스

        public BaseNode CurrentBase => _currentBase;
        public bool IsSafe { get; private set; } = true;

        // --- 초기화 및 배치 ---
        [Header("Advanced Runner")]
        [SerializeField] private float _baseReachThreshold = 3.0f; // 베이스 도착 3m 전부터 오버런 판단

        // --- 행동 1: 리드 (Take Lead) ---
        public void TakeLead(float leadDistance)
        {
            if (_currentBase == null) return;

            // 다음 베이스 방향 확인
            var fieldMgr = GameRoot.Instance.GetManager<Bash.Framework.Managers.FieldManager>();
            var nextBase = fieldMgr.CurrentField.GetNextBase(_currentBase.BaseType);

            if (nextBase != null)
            {
                // 현재 베이스에서 다음 베이스 방향으로 leadDistance만큼 이동
                Vector3 direction = (nextBase.GetTouchPosition() - _currentBase.GetTouchPosition()).normalized;
                Vector3 leadPos = _currentBase.GetRunnerPosition() + (direction * leadDistance);

                // 이동 명령 (느린 속도로 슬금슬금)
                Movement.MoveTo(leadPos);
                LookAt(Vector3.zero); // 시선은 투수(홈) 고정
            }
        }

        // --- 행동 2: 오버런 체크용 정보 제공 ---

        // 다음 베이스까지 남은 거리
        public float GetDistanceToTarget()
        {
            if (_targetBase == null) return 0f;
            return Vector3.Distance(transform.position, _targetBase.GetTouchPosition());
        }

        // 현재 오버런 판단 구간(베이스 근처)에 진입했는가?
        public bool IsApproachingBase()
        {
            if (_targetBase == null) return false;
            float dist = GetDistanceToTarget();
            return dist <= _baseReachThreshold && dist > 0.5f; // 너무 가까우면(0.5f) 이미 도착한 것
        }
    
        /// <summary>
        /// 주자를 특정 베이스에 강제 배치 (대주자, 안타 후 세팅 등)
        /// </summary>
        public void PlaceOnBase(BaseNode baseNode)
        {
            if (baseNode == null) return;

            _currentBase = baseNode;
            _targetBase = null;
            IsSafe = true;

            // 물리적 위치 이동 (RunnerSpot)
            Movement.Stop();
            transform.position = baseNode.GetRunnerPosition();

            // 홈 방향(0,0,0) 바라보기
            LookAt(Vector3.zero);
        }

        // --- 행동 명령 (Action) ---

        public void RunToNextBase()
        {
            if (_currentBase == null) return;

            // 1. FieldManager를 통해 다음 베이스 찾기
            var fieldMgr = GameRoot.Instance.GetManager<FieldManager>();
            if (fieldMgr == null || fieldMgr.CurrentField == null) return;

            // 현재 베이스의 다음 베이스 가져오기
            _targetBase = fieldMgr.CurrentField.GetNextBase(_currentBase.BaseType);

            if (_targetBase != null)
            {
                IsSafe = false; // 베이스를 떠났으므로 위험 상태
                Movement.MoveTo(_targetBase.GetTouchPosition()); // 베이스 터치 지점으로 이동

                Debug.Log($"[Runner] Start running to {_targetBase.BaseType}");
            }
        }

        public void ReturnToBase()
        {
            if (_currentBase != null)
            {
                _targetBase = null;
                Movement.MoveTo(_currentBase.GetRunnerPosition());
            }
        }

        public void LookAt(Vector3 target)
        {
            Vector3 dir = (target - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero) transform.rotation = Quaternion.LookRotation(dir);
        }

        // --- 물리 상호작용 (Base Touch) ---

        private void OnTriggerEnter(Collider other)
        {
            // 목표하던 베이스에 닿았는지 확인
            if (_targetBase != null)
            {
                var baseNode = other.GetComponent<BaseNode>();
                if (baseNode != null && baseNode == _targetBase)
                {
                    OnTouchBase(baseNode);
                }
            }
        }

        private void OnTouchBase(BaseNode baseNode)
        {
            Debug.Log($"[Runner] Touched Base: {baseNode.BaseType}");

            // 이전 베이스 비우기
            if (_currentBase != null) _currentBase.ClearOccupant();

            _currentBase = baseNode;
            _targetBase = null;
            IsSafe = true;

            // [New] 새 베이스 점유 등록
            _currentBase.SetOccupant(this);

            Movement.Stop();
            //일단은 멈추는 로직
            transform.position = baseNode.GetRunnerPosition();
            LookAt(Vector3.zero);
        }
    }
}