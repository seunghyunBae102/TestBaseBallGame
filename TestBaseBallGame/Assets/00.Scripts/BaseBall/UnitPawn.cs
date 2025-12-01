using UnityEngine;
using Bash.Framework.Core;

namespace Bash.Core.Unit
{
    [RequireComponent(typeof(UnitMovementCompo))]
    public class UnitPawn : ActorNode
    {
        // 내 몸에 달린 기능들 캐싱
        public UnitMovementCompo Movement { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Movement = GetCompo<UnitMovementCompo>();
        }

        // --- 공통 행동 명령 (Controller가 호출함) ---

        public void MoveToLocation(Vector3 pos)
        {
            Movement.MoveTo(pos);
        }

        public void StopMovement()
        {
            Movement.Stop();
        }

        // 시선 처리
        public void LookAt(Vector3 target)
        {
            Vector3 dir = (target - transform.position).normalized;
            dir.y = 0; // 수평 유지
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }
    }
}