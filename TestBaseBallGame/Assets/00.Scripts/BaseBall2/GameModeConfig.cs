// Scripts/Game/Baseball/Config/GameModeConfig.cs
using UnityEngine;
using Bash.Framework.Core;
using Bash.Game.Baseball.Shared;

namespace Bash.Game.Baseball.Config
{
    /// <summary>
    /// 연장전 규칙.
    /// </summary>
    public enum ExtraInningRule
    {
        /// <summary>
        /// 연장전 없음. 정규 이닝 이후에는 무승부 또는 즉시 종료.
        /// </summary>
        None,

        /// <summary>
        /// 정해진 이닝 수까지 연장 후, 동점이면 무승부.
        /// </summary>
        TieAfterMax,

        /// <summary>
        /// 승부가 날 때까지 계속 연장.
        /// </summary>
        Infinite
    }

    /// <summary>
    /// 경기 규칙/모드 설정 ScriptableObject.
    /// - 한 경기의 이닝 수, 카운트 규칙, 콜드게임, 연장 규칙 등을 정의한다.
    /// - 룰 모듈 및 State 모듈에서 참조하는 "상수 설정" 레이어.
    /// </summary>
    [CreateAssetMenu(menuName = "Baseball/Game Mode Config", fileName = "GameModeConfig")]
    public class GameModeConfig : ScriptableConfig
    {
        [Header("Innings")]
        [Min(1)]
        public int totalInnings = 9;

        [Tooltip("정규 이닝 이후 연장 규칙.")]
        public ExtraInningRule extraInningRule = ExtraInningRule.TieAfterMax;

        [Tooltip("연장 이닝을 최대 몇 이닝까지 허용하는지 (extraInningRule이 TieAfterMax일 때 사용).")]
        [Min(0)]
        public int maxExtraInnings = 3;

        [Header("Count Rules")]
        [Tooltip("볼 4개면 볼넷.")]
        [Min(1)]
        public int maxBalls = 4;

        [Tooltip("스트라이크 3개면 삼진.")]
        [Min(1)]
        public int maxStrikes = 3;

        [Tooltip("이닝(초/말)당 아웃 수 (보통 3).")]
        [Min(1)]
        public int outsPerHalfInning = 3;

        [Header("Mercy Rule (콜드게임)")]
        [Tooltip("콜드게임(점수차에 따라 조기 종료) 적용 여부.")]
        public bool useMercyRule = false;

        [Tooltip("콜드게임으로 종료되는 점수 차 (예: 10점 차).")]
        [Min(1)]
        public int mercyRunDifference = 10;

        [Tooltip("콜드게임을 적용하기 시작하는 이닝 번호 (예: 5회부터).")]
        [Min(1)]
        public int mercyRuleStartInning = 5;

        [Header("Tie / Misc")]
        [Tooltip("경기 종료 시 동점 허용 여부 (리그 규정에 따라 다를 수 있음).")]
        public bool allowTie = false;

        [Tooltip("연장전에서 홈/원정 공격 순서를 바꾸는 특수 규칙을 허용할지 여부 (보통 false).")]
        public bool allowSideSwapInExtra = false;

        /// <summary>
        /// 현재 이닝/점수 상황이 콜드게임 조건에 해당하는지 체크한다.
        /// 실제 MatchEnd 판정은 별도 룰 모듈에서 한다.
        /// </summary>
        public bool IsMercyConditionMet(int inningNumber, TeamSide leadingTeam, int homeScore, int awayScore)
        {
            if (!useMercyRule)
                return false;

            if (inningNumber < mercyRuleStartInning)
                return false;

            int diff = leadingTeam == TeamSide.Home
                ? homeScore - awayScore
                : awayScore - homeScore;

            return diff >= mercyRunDifference;
        }

        /// <summary>
        /// 추가 이닝이 더 가능한지 여부를 반환한다.
        /// extraInningRule과 maxExtraInnings, allowTie에 따라 판단한다.
        /// </summary>
        public bool CanPlayMoreExtraInnings(int currentInningNumber)
        {
            if (currentInningNumber <= totalInnings)
                return true; // 아직 정규 이닝

            int extraInningsPlayed = currentInningNumber - totalInnings;

            switch (extraInningRule)
            {
                case ExtraInningRule.None:
                    return false;

                case ExtraInningRule.TieAfterMax:
                    return extraInningsPlayed < maxExtraInnings;

                case ExtraInningRule.Infinite:
                    return true;

                default:
                    return false;
            }
        }
    }
}
