// Scripts/Game/Baseball/Config/TeamDataSO.cs
using UnityEngine;
using Bash.Framework.Core;
using Bash.Game.Baseball.Shared;

namespace Bash.Game.Baseball.Config
{
    /// <summary>
    /// 수비 포지션 타입.
    /// 실제 포지션 배치/선발 라인업은 TeamDataSO에서 정의한다.
    /// </summary>
    public enum PositionType
    {
        None = 0,
        Pitcher,
        Catcher,
        FirstBase,
        SecondBase,
        ThirdBase,
        ShortStop,
        LeftField,
        CenterField,
        RightField,
        DesignatedHitter
    }

    /// <summary>
    /// 선발 라인업의 한 슬롯.
    /// </summary>
    [System.Serializable]
    public struct LineupSlot
    {
        [Tooltip("이 슬롯에 배치되는 선수 데이터.")]
        public PlayerDataSO player;

        [Tooltip("타순 인덱스 (0 기반: 0=1번 타자, 8=9번 타자).")]
        [Range(0, 8)]
        public int battingOrderIndex;

        [Tooltip("수비 포지션.")]
        public PositionType position;

        [Tooltip("선발투수인지 여부.")]
        public bool isStartingPitcher;
    }

    /// <summary>
    /// 팀(홈/원정) 구성 정보 ScriptableObject.
    /// - 로스터(전체 선수 목록)
    /// - 선발 라인업
    /// - 벤치, 선발 로테이션, 불펜 등
    /// </summary>
    [CreateAssetMenu(menuName = "Baseball/Team Data", fileName = "TeamData")]
    public class TeamDataSO : ScriptableConfig
    {
        [Header("Identity")]
        [Tooltip("내부 식별용 팀 ID.")]
        public string teamId;

        [Tooltip("UI 등에 표시할 팀 이름.")]
        public string teamName;

        [Tooltip("스코어보드 등에 표시할 짧은 코드 (예: BOS, NYY).")]
        public string shortCode;

        [Header("Default Side")]
        [Tooltip("이 팀이 기본적으로 홈/원정 중 어느 쪽인지 (리그 규정에 따라 다를 수 있음).")]
        public TeamSide defaultSide = TeamSide.Home;

        [Header("Roster")]
        [Tooltip("팀에 소속된 전체 선수 목록.")]
        public PlayerDataSO[] roster;

        [Header("Starting Lineup")]
        [Tooltip("선발 라인업 (보통 9명). battingOrderIndex 0~8 사용 권장.")]
        public LineupSlot[] startingLineup;

        [Header("Bench / Pitchers")]
        [Tooltip("야수 벤치.")]
        public PlayerDataSO[] benchPlayers;

        [Tooltip("선발 로테이션 후보.")]
        public PlayerDataSO[] startingRotation;

        [Tooltip("불펜 투수 목록.")]
        public PlayerDataSO[] bullpen;

        /// <summary>
        /// 주어진 타순 인덱스(0 기반)에 해당하는 라인업 슬롯을 가져온다.
        /// - 없으면 null을 반환한다.
        /// </summary>
        public LineupSlot? GetLineupSlot(int battingOrderIndex)
        {
            if (startingLineup == null) return null;

            for (int i = 0; i < startingLineup.Length; i++)
            {
                if (startingLineup[i].battingOrderIndex == battingOrderIndex)
                    return startingLineup[i];
            }

            return null;
        }

        /// <summary>
        /// 특정 포지션에 배치된 선발 선수를 반환한다. 없으면 null.
        /// </summary>
        public PlayerDataSO GetStartingPlayerByPosition(PositionType position)
        {
            if (startingLineup == null) return null;

            for (int i = 0; i < startingLineup.Length; i++)
            {
                if (startingLineup[i].position == position)
                    return startingLineup[i].player;
            }

            return null;
        }

        /// <summary>
        /// 선발 라인업에서 선발투수로 표시된 선수 반환.
        /// 없으면 null.
        /// </summary>
        public PlayerDataSO GetStartingPitcher()
        {
            if (startingLineup != null)
            {
                for (int i = 0; i < startingLineup.Length; i++)
                {
                    if (startingLineup[i].isStartingPitcher && startingLineup[i].player != null)
                        return startingLineup[i].player;
                }
            }

            // startingRotation에 후보가 있다면 첫 번째를 반환하는 것도 하나의 정책일 수 있음.
            if (startingRotation != null && startingRotation.Length > 0)
                return startingRotation[0];

            return null;
        }
    }
}
