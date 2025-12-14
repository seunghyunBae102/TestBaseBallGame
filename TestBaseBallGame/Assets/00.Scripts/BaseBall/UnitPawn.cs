using UnityEngine;
using Bash.Framework.Core;
using Bash.Core.Data;

namespace Bash.Core.Unit
{
    public class UnitPawn : ActorNode
    {
        public PlayerDTO Data { get; private set; }
        public bool IsHomeTeam { get; private set; }

        public UnitMovementCompo Movement { get; private set; }
        public UnitStatCompo Stat { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Movement = GetCompo<UnitMovementCompo>();
            Stat = GetCompo<UnitStatCompo>();
        }

        // [New] 데이터 초기화
        public virtual void Initialize(PlayerDTO data, bool isHomeTeam)
        {
            Data = data;
            IsHomeTeam = isHomeTeam;

            if (Data != null)
            {
                gameObject.name = $"{Data.Name}_{Data.ID}";
                if (Stat != null) Stat.ApplyStats(Data.BaseStats);
            }
        }

        // [Fix] 시선 처리 메서드 추가 (RosterManager 등에서 호출)
        public void LookAt(Vector3 target)
        {
            Vector3 dir = (target - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }
    }
}