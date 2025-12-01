using Bash.Core.Unit;
using Bash.Framework.Core;
using UnityEngine;

namespace Bash.Core.GamePlay
{
    public class FielderPawn : UnitPawn
    {
        [Header("Fielder Sockets")]
        [SerializeField] private Transform _gloveHand; // 공 잡는 손 (왼손)
        [SerializeField] private Transform _throwHand; // 던지는 손 (오른손)

        // 공이 내 글러브에 붙게 함
        public void AttachBallToGlove(ActorNode ballNode)
        {
            if (_gloveHand == null) return;

            // 공의 물리 끄기 (BallPhysicsCompo 메서드 필요)
            // ballNode.GetCompo<BallPhysicsCompo>().SetKinematic(true); 

            // 위치 이동
            ballNode.transform.SetParent(_gloveHand);
            ballNode.transform.localPosition = Vector3.zero;
            ballNode.transform.localRotation = Quaternion.identity;
        }
    }
}