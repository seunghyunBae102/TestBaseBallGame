// Scripts/Game/Baseball/Ball/BallBehaviourSO.cs
using UnityEngine;

namespace Bash.Game.Baseball.Ball
{
    /// <summary>
    /// 공의 "기본 궤적"을 정의하는 ScriptableObject 베이스.
    /// - 직구/커브/싱커/타구 포물선 등은 이 클래스를 상속해서 구현한다.
    /// - 야구 룰은 모르고, 오직 위치/속도/회전만 조작한다.
    /// </summary>
    public abstract class BallBehaviourSO : ScriptableObject
    {
        /// <summary>
        /// 공이 스폰될 때 한 번 호출.
        /// </summary>
        public virtual void Initialize(ref BallRuntimeState state, in BallSpawnContext context)
        {
            state.Position = context.spawnPosition;
            state.Velocity = context.initialVelocity;
            state.SpinAxis = context.spinAxis;
            state.SpinRate = context.spinRate;
            state.Phase = context.initialPhase;
            state.LifeTime = 0f;
            state.IsGrounded = false;
            state.IsInPlayArea = true;
            state.MarkForDespawn = false;
        }

        /// <summary>
        /// 매 프레임(혹은 FixedUpdate)마다 호출.
        /// 이 안에서 state.Position/Velocity 등을 업데이트한다.
        /// </summary>
        public virtual void UpdateBehaviour(ref BallRuntimeState state, float deltaTime)
        {
            // 기본 구현: 단순 위치 업데이트만.
            state.Position += state.Velocity * deltaTime;
            state.LifeTime += deltaTime;
        }

        /// <summary>
        /// (선택) 디버그용 궤적 그리기 등.
        /// </summary>
        public virtual void DrawGizmos(in BallRuntimeState state)
        {
        }
    }
}
