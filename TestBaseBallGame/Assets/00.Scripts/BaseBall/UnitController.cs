using UnityEngine;
using Bash.Framework.Core;

namespace Bash.Core.Unit
{
    public abstract class UnitController : ActorCompo
    {
        // 내가 제어 중인 육체
        protected UnitPawn ControlledPawn;

        // 빙의 (제어권 획득)
        public virtual void Possess(UnitPawn pawn)
        {
            ControlledPawn = pawn;
        }

        public virtual void UnPossess()
        {
            ControlledPawn = null;
        }

        // 유효성 검사 헬퍼
        protected bool HasPawn => ControlledPawn != null;
    }
}   