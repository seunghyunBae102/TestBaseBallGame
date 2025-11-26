// Scripts/Game/Baseball/Ball/BallConfigSetSO.cs
using UnityEngine;

namespace Bash.Game.Baseball.Ball
{
    /// <summary>
    /// 한 구종/타구 타입에 대응하는 Behaviour + Skill 세트 프리셋.
    /// Pitch/Hit 모듈이 이 설정을 참조해서 BallFactory에 넘길 수 있다.
    /// </summary>
    [CreateAssetMenu(menuName = "Baseball/Ball Config Set", fileName = "BallConfigSet")]
    public class BallConfigSetSO : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("내부 식별용 ID (예: four_seam_fastball, curve_A 등).")]
        public string presetId;

        [Header("Core Behaviour")]
        public BallBehaviourSO behaviour;

        [Header("Skills")]
        public BallSkillSO[] skills;

        [Header("Base Parameters")]
        [Tooltip("기본 속도 계수 (투구/타구 모듈에서 initialVelocity 만들 때 사용).")]
        public float baseSpeed = 30f;

        [Tooltip("기본 회전 속도 (rpm 등, Behaviour 해석 방식에 따름).")]
        public float baseSpinRate = 1500f;

        [Tooltip("타구용: 발사 각도 범위 (deg). x=최소, y=최대.")]
        public Vector2 launchAngleRange;

        [Tooltip("타구용: 좌우 방향 각도 범위 (deg). x=최소, y=최대.")]
        public Vector2 sprayAngleRange;
    }
}
