using UnityEngine;
using Bash.Framework.Core;

namespace Bash.Core.Ball
{
    public class BallNode : ActorNode
    {
        // 공의 물리적 상태
        public Vector3 Velocity { get; set; }
        public float Radius { get; private set; } = 0.1f; // 야구공 반지름 (약 7cm)

        // 현재 공이 "누구"의 소유인지 (투수? 타자?)
        // public ActorNode LastOwner { get; set; } 

        protected override void Awake()
        {
            base.Awake();
            // 필요 시 태그나 레이어 설정
            gameObject.layer = LayerMask.NameToLayer("Ball");
        }

        /// <summary>
        /// 공을 발사하는 간편 메서드
        /// </summary>
        public void Launch(Vector3 dir, float speed)
        {
            Velocity = dir.normalized * speed;

            // 물리 엔진 리셋 등 필요한 로직 호출
            var phys = GetCompo<BallPhysicsCompo>();
            if (phys != null) phys.ResetSimulation();
        }
    }
}