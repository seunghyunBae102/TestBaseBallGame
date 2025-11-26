// Scripts/Game/Baseball/Events/PlayEvents.cs
using UnityEngine;

namespace Bash.Game.Baseball.Events
{
    /// <summary>
    /// Raw 피치 결과를 기반으로 PitchOutcome 계산을 요청하는 이벤트.
    /// - PitchZoneDetector/충돌 판정 시스템이 발행한다.
    /// </summary>
    public struct PitchOutcomeRequest
    {
        public bool BatterSwung;
        public bool MadeContact;
        public bool IsInStrikeZone;
        public bool IsFoul;
        public bool HitBatter;
    }

    /// <summary>
    /// 타구 결과(HitBallResult) 계산을 요청하는 이벤트.
    /// - 필드/벽/파울라인 등 충돌을 판정하는 시스템이 발행한다.
    /// </summary>
    public struct HitResultRequest
    {
        public bool IsFair;
        public bool IsHomeRun;
        public bool BallCaughtInAir;
        public bool LandedInFoulTerritory;
        public float Distance;
    }
}
