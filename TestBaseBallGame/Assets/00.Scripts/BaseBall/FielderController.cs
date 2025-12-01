using UnityEngine;
using Bash.Core.Unit;

namespace Bash.Core.GamePlay
{
    public class FielderController : UnitController
    {
        // Pawn을 FielderPawn으로 캐스팅해서 사용
        private FielderPawn Fielder => ControlledPawn as FielderPawn;

        // 외부(GameRule 혹은 Director)에서 호출
        public void OrderChase(Vector3 destination)
        {
            if (!HasPawn) return;

            Debug.Log($"[FielderAI] Chasing ball to {destination}");
            Fielder.MoveToLocation(destination);
        }

        public void OrderCatch()
        {
            if (!HasPawn) return;
            // 애니메이션 재생 트리거 등 (추후 구현)
            Debug.Log("[FielderAI] Attempting Catch!");
        }
    }
}