// Scripts/Game/Baseball/Config/PlayerDataSO.cs
using UnityEngine;


    [CreateAssetMenu(menuName = "Baseball/PlayerData")]
    public class PlayerDataSO : ScriptableConfig
    {
        [Header("�⺻ ����")]
        public string displayName;
        public PlayerRole role;

        [Header("�ɷ�ġ")]
        public PlayerStats stats;

        [Header("���Ŀ�: ��ų ��� ��")]
        public ScriptableObject[] skills; // ���߿� PlayerSkillSO�� ��ü
    }

    [CreateAssetMenu(menuName = "Baseball/TeamData")]
    public class TeamDataSO : ScriptableConfig
    {
        [Header("�� ����")]
        public string teamName;
        public string shortName; // UI�� ��Ī

        [Header("���ξ� (Ÿ�� 1~9�� �������)")]
        public PlayerDataSO[] battingOrder; // ���� 9 ����

        [Header("������")]
        public PlayerDataSO startingPitcher;
        public PlayerDataSO[] bullpen; // ����

        [Header("���� ������ ���� (���� ����)")]
        public PlayerDataSO firstBaseman;
        public PlayerDataSO secondBaseman;
        public PlayerDataSO shortStop;
        public PlayerDataSO thirdBaseman;
        public PlayerDataSO leftFielder;
        public PlayerDataSO centerFielder;
        public PlayerDataSO rightFielder;
        public PlayerDataSO catcher;
    }


//public enum PlayerRole
//{
//    Unknown,
//    Pitcher,
//    Batter,
//    Fielder,
//    TwoWay
//}

//[System.Serializable]
//public struct BattingStats
//{
//    public float contact;     // ���ߴ� �ɷ�
//    public float power;       // ��Ÿ�
//    public float discipline;  // ��/��Ʈ ����
//}

//[System.Serializable]
//public struct PitchingStats
//{
//    public float velocity;     // ����
//    public float control;      // ����
//    public float breakAmount;  // ��ȭ��
//}

//[System.Serializable]
//public struct RunningStats
//{
//    public float speed; // ��
//    public float steal; // ���� �ɷ�
//}

//[System.Serializable]
//public struct FieldingStats
//{
//    public float range;       // ���� ����
//    public float armStrength; // �۱� ��
//    public float hands;       // ���� ������
//}

//[System.Serializable]
//public struct PlayerStats
//{
//    public BattingStats batting;
//    public PitchingStats pitching;
//    public RunningStats running;
//    public FieldingStats fielding;
//}
