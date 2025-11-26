// Scripts/Game/Baseball/Ball/BallSkillSO.cs
using UnityEngine;

namespace Bash.Game.Baseball.Ball
{
    /// <summary>
    /// 공에 추가 효과/스킬을 부여하는 ScriptableObject 베이스.
    /// - 하나의 공에 여러 BallSkillSO가 동시에 붙을 수 있다.
    /// - Behaviour 위에 얹는 "추가 레이어"라고 보면 된다.
    /// </summary>
    public abstract class BallSkillSO : ScriptableObject
    {
        /// <summary>
        /// 공이 스폰될 때 호출.
        /// Behaviour.Initialize가 끝난 뒤 호출되는 것이 일반적이다.
        /// </summary>
        public virtual void OnBallSpawned(ref BallRuntimeState state, in BallSpawnContext context)
        {
        }

        /// <summary>
        /// 매 프레임(혹은 FixedUpdate)마다 호출.
        /// Behaviour.UpdateBehaviour 이후에 호출되는 것이 일반적이다.
        /// </summary>
        public virtual void OnBallUpdated(ref BallRuntimeState state, float deltaTime)
        {
        }

        /// <summary>
        /// 공의 Phase가 변경될 때 호출.
        /// (예: Pitched → Hit, Hit → Dead 등)
        /// </summary>
        public virtual void OnPhaseChanged(ref BallRuntimeState state, BallPhase previousPhase, BallPhase newPhase)
        {
        }

        /// <summary>
        /// 충돌 처리 직전에 호출.
        /// 필요한 경우 state를 수정할 수 있다.
        /// </summary>
        public virtual void OnBeforeCollision(ref BallRuntimeState state, Collider other, Vector3 hitPoint)
        {
        }

        /// <summary>
        /// 충돌 처리 직후에 호출.
        /// 여기서 MarkForDespawn 등을 설정해도 된다.
        /// </summary>
        public virtual void OnAfterCollision(ref BallRuntimeState state, Collider other, Vector3 hitPoint)
        {
        }

        /// <summary>
        /// 공이 디스폰(풀 반환/Destroy)되기 직전에 호출.
        /// </summary>
        public virtual void OnBallDespawned(in BallRuntimeState finalState)
        {
        }
    }
}
