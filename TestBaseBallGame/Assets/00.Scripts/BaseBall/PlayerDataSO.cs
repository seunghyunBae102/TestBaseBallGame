using UnityEngine;

namespace Bash.Core.Data
{
    [CreateAssetMenu(fileName = "NewPlayer", menuName = "Bash/Player Data (Light)")]
    public class PlayerDataSO : ScriptableObject
    {
        public PlayerDTO Data;
    }
}