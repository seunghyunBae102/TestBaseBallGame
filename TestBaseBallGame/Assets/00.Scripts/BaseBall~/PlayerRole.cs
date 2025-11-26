// Scripts/Game/Baseball/Config/PlayerTypes.cs

    public enum PlayerRole
    {
        Unknown,
        Pitcher,
        Batter,   // �ַ� Ÿ�ݿ�(����Ÿ�� ��)
        Fielder,  // �����ַ� �߽�
        TwoWay    // ��Ÿ ��� ��
    }

/// <summary>
/// ���� ���� ���� �ɷ�ġ. ���߿� ����ȭ�ص� ��.
/// </summary>
[System.Serializable]
public struct BattingStats
{
    public float contact;     // ���ߴ� �ɷ�
    public float power;       // ��Ÿ�
    public float discipline;  // �� ������ (�� ��󳻱�)
}

[System.Serializable]
    public struct PitchingStats
    {
        public float velocity;     // ����
        public float control;      // ������
        public float breakAmount;  // ��ȭ��
    }

    [System.Serializable]
    public struct RunningStats
    {
        public float speed;        // �ַ� �ӵ�
        public float steal;        // ���� �ɷ�
    }

    [System.Serializable]
    public struct FieldingStats
    {
        public float range;        // ������ ����
        public float armStrength;  // �۱� ��
        public float hands;        // ���� ������
    }

[System.Serializable]
public struct PlayerStats
{
    public BattingStats batting;
    public PitchingStats pitching;
    public RunningStats running;
    public FieldingStats fielding;
}



