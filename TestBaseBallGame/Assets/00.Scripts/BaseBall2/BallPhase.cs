// Scripts/Game/Baseball/Ball/BallCoreTypes.cs
using UnityEngine;
using Bash.Game.Baseball.Shared;
using Bash.Framework.Node;

namespace Bash.Game.Baseball.Ball
{
    /// <summary>
    /// 공의 현재 단계(라이프 사이클).
    /// </summary>
    public enum BallPhase
    {
        None = 0,
        Pitched,   // 투수 → 포수/타자
        Hit,       // 타구
        Thrown,    // 수비/주자의 송구
        Caught,    // 누군가가 잡고 들고 있는 상태
        Dead       // 더 이상 플레이에 사용되지 않음 (파울 회수, 홈런 후 등)
    }

    /// <summary>
    /// 공이 스폰될 때 필요한 초기 정보.
    /// Pitch/Hit/Throw 모듈이 채워서 BallFactory에 넘긴다.
    /// </summary>
    public struct BallSpawnContext
    {
        public Vector3 spawnPosition;
        public Vector3 initialVelocity;

        public Vector3 spinAxis;
        public float spinRate;

        public BallPhase initialPhase;

        /// <summary>공을 만든 주체 (투수/타자/수비수 등).</summary>
        public ActorNode ownerActor;

        /// <summary>공을 만든 팀.</summary>
        public TeamSide ownerTeam;

        /// <summary>공의 위력/강도 같은 메타 값 (타구 품질/구속 계수 등).</summary>
        public float powerFactor;

        /// <summary>타구일 때 빗맞음/정타 여부 같은 품질 지표.</summary>
        public float contactQuality;
    }

    /// <summary>
    /// 공의 현재 런타임 상태.
    /// BallCore 내부에서 관리하고 Behaviour/Skill에서 읽고/수정한다.
    /// </summary>
    public struct BallRuntimeState
    {
        public Vector3 Position;
        public Vector3 Velocity;

        public Vector3 SpinAxis;
        public float SpinRate;

        public BallPhase Phase;

        public float LifeTime;

        public bool IsGrounded;
        public bool IsInPlayArea;

        /// <summary>
        /// true면 다음 프레임에 제거해도 된다는 의미.
        /// (디스폰/풀 반환 트리거)
        /// </summary>
        public bool MarkForDespawn;
    }
}
