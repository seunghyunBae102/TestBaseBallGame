// Scripts/Game/Baseball/Config/PlayerDataSO.cs
using UnityEngine;
using Bash.Framework.Core;

namespace Bash.Game.Baseball.Config
{
    /// <summary>
    /// 선수의 주 역할(포지션이 아니라 역할 개념).
    /// 실제 수비 포지션은 TeamDataSO의 LineupSlot에서 정의한다.
    /// </summary>
    public enum PlayerRole
    {
        Unknown = 0,
        Hitter,         // 야수/타자 위주
        Pitcher,        // 투수 위주
        TwoWay,         // 투타 겸업
        PinchRunner,    // 대주자 전문
        Utility         // 여러 포지션 소화
    }

    public enum BattingSide
    {
        Right,
        Left,
        Switch
    }

    public enum ThrowingSide
    {
        Right,
        Left
    }

    #region Stats structs

    [System.Serializable]
    public struct BattingStats
    {
        [Range(0, 100)] public int contact;
        [Range(0, 100)] public int power;
        [Range(0, 100)] public int eye;          // 볼넷/삼진 경향
        [Range(0, 100)] public int clutch;       // 득점권/핵심 상황에서의 능력
        [Range(0, 100)] public int discipline;   // 헛스윙 줄이기, 존 관리
    }

    [System.Serializable]
    public struct PitchingStats
    {
        [Range(0, 100)] public int velocity;      // 구속
        [Range(0, 100)] public int control;       // 제구력
        [Range(0, 100)] public int movement;      // 변화량
        [Range(0, 100)] public int stamina;       // 투구 이닝/투구수 버티는 능력
        [Range(0, 100)] public int composure;     // 위기 상황 멘탈
    }

    [System.Serializable]
    public struct RunningStats
    {
        [Range(0, 100)] public int speed;         // 순수 스피드
        [Range(0, 100)] public int acceleration;  // 출발, 스타트
        [Range(0, 100)] public int steal;         // 도루능력
        [Range(0, 100)] public int awareness;     // 주루 상황 판단
    }

    [System.Serializable]
    public struct FieldingStats
    {
        [Range(0, 100)] public int range;         // 수비 범위
        [Range(0, 100)] public int armStrength;   // 송구 힘
        [Range(0, 100)] public int armAccuracy;   // 송구 정확도
        [Range(0, 100)] public int hands;         // 글러브 핸들링, 에러 빈도
        [Range(0, 100)] public int reaction;      // 타구 반응속도
    }

    [System.Serializable]
    public struct PlayerStats
    {
        public BattingStats batting;
        public PitchingStats pitching;
        public RunningStats running;
        public FieldingStats fielding;
    }

    #endregion

    /// <summary>
    /// 선수 1명에 대한 기본 데이터 ScriptableObject.
    /// - 이름, 등번호, 주 역할
    /// - 타/투 손
    /// - 스탯
    /// - (선택) 스킬 레퍼런스 배열
    /// </summary>
    [CreateAssetMenu(menuName = "Baseball/Player Data", fileName = "PlayerData")]
    public class PlayerDataSO : ScriptableConfig
    {
        [Header("Identity")]
        [Tooltip("내부 식별용 ID (저장/모딩용).")]
        public string playerId;

        [Tooltip("UI 등에 표시할 이름.")]
        public string displayName;

        [Tooltip("등번호.")]
        public int uniformNumber = 0;

        [Header("Profile")]
        public PlayerRole primaryRole = PlayerRole.Hitter;
        public BattingSide battingSide = BattingSide.Right;
        public ThrowingSide throwingSide = ThrowingSide.Right;

        [Header("Stats")]
        public PlayerStats stats;

        [Header("Skills / Traits")]
        [Tooltip("선수의 특수 스킬 / 특성. 이후 구체 타입(BaseballSkillSO 등)으로 교체 가능.")]
        public ScriptableObject[] skills;
    }
}
