// Scripts/Game/Baseball/Play/PlaySharedTypes.cs
using Bash.Framework.Node;
using Bash.Game.Baseball.Ball;
using Bash.Game.Baseball.Config;
using Bash.Game.Baseball.Events;
using Bash.Game.Baseball.Shared;
using UnityEngine;

namespace Bash.Game.Baseball.Play
{
    /// <summary>
    /// 한 번의 투구에 대한 명령/설정 값.
    /// - 투수/팀/구종 프리셋/노리는 위치/힘 등을 포함한다.
    /// </summary>
    public struct PitchCommand
    {
        public TeamSide pitcherTeam;
        public ActorNode pitcherActor;

        /// <summary>어떤 구종/타구 프리셋을 사용할지.</summary>
        public BallConfigSetSO ballPreset;

        /// <summary>볼 릴리즈 위치(투수 손).</summary>
        public Vector3 releasePosition;

        /// <summary>노리고 있는 목표 지점(스트라이크 존 내/밖).</summary>
        public Vector3 targetPoint;

        /// <summary>투구 힘 계수(1.0 = 기본, 1.2 = 더 강하게 등).</summary>
        public float powerFactor;

        /// <summary>고의4구, 특수 연출 등.</summary>
        public bool isIntentionalBall;
    }

    /// <summary>
    /// 타자의 스윙 정보를 담는 명령값.
    /// - BatSwingModule가 입력/애니메이션을 모아서 만든다.
    /// </summary>
    public struct SwingCommand
    {
        public TeamSide batterTeam;
        public ActorNode batterActor;

        /// <summary>스윙했는지 여부 (false면 그냥 지켜본 것).</summary>
        public bool didSwing;

        /// <summary>번트 시도인지 여부.</summary>
        public bool isBunt;

        /// <summary>배트 스윙 평면의 노멀 벡터 (대략적인 스윙 방향/궤도).</summary>
        public Vector3 swingPlaneNormal;

        /// <summary>투구 기준 상대 타이밍 (0=너무 빠름, 0.5=적절, 1=너무 늦음 등).</summary>
        public float swingTiming;

        /// <summary>상/중/하 존에서 어느 높이인지 (0=아래, 1=위쪽).</summary>
        public float swingHeightRatio;
    }

    /// <summary>
    /// 타구용 공을 만들 때 필요한 추가 정보.
    /// - BallSpawnContext + 타구 품질/각도/타입.
    /// </summary>
    public struct HitBallSpawnContext
    {
        /// <summary>5단계 BallCore에서 쓰는 기본 컨텍스트.</summary>
        public BallSpawnContext baseContext;

        /// <summary>타구 타입(예: 라인드라이브/플라이/그라운드 등)을 예측해 둔 값.</summary>
        public HitType predictedHitType;

        /// <summary>타구 출구 속도 (m/s).</summary>
        public float exitVelocity;

        /// <summary>타구 방향 단위 벡터.</summary>
        public Vector3 launchDirection;

        /// <summary>발사각 (deg, 수평 기준).</summary>
        public float launchAngle;

        /// <summary>좌/우 방향 각도 (deg, 라인 기준 각).</summary>
        public float sprayAngle;

        /// <summary>미트 정확도/빗맞음 정도.</summary>
        public float contactQuality;
    }
}
