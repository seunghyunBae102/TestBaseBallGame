// Scripts/Game/Baseball/Config/GameModeConfig.cs
using UnityEngine;


    [CreateAssetMenu(menuName = "Baseball/GameModeConfig")]
    public class GameModeConfig : ScriptableConfig
    {
        [Header("�⺻ �̴� ��")]
        public int regularInnings = 9;

        [Header("�߰� �̴� ��� ����")]
        public bool allowExtraInnings = true;

        [Header("ī��Ʈ ����")]
        public int maxBalls = 4;
        public int maxStrikes = 3;
        public int maxOuts = 3;

        [Header("�� ���� ��")]
        public TeamSide firstOffense = TeamSide.Away; // ���� �������� ���� ����
    }


