using System.Collections.Generic;
using UnityEngine;

namespace Bash.Core.Data
{
    [CreateAssetMenu(fileName = "NewTeam", menuName = "Bash/Team Data")]
    public class TeamDataSO : ScriptableObject
    {
        public string TeamName;
        public Color TeamColor = Color.white;

        [Header("Roster")]
        public List<PlayerDataSO> BattingOrder; // 1~9번 타자

        [Header("Defense")]
        public PlayerDataSO Pitcher;
        public PlayerDataSO Catcher;
        public PlayerDataSO FirstBaseman;
        public PlayerDataSO SecondBaseman;
        public PlayerDataSO ThirdBaseman;
        public PlayerDataSO ShortStop;
        public PlayerDataSO LeftFielder;
        public PlayerDataSO CenterFielder;
        public PlayerDataSO RightFielder;
    }
}