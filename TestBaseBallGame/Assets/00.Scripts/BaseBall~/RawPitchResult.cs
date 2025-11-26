// Scripts/Game/Baseball/Events/PitchHitEvents.cs


    /// <summary>
    /// ���� �� �� ����� �Ǵܿ� ����ϴ� Raw ������.
    /// </summary>
    public struct RawPitchResult
    {
        public bool IsInStrikeZone;  // �� ��� ����
        public bool BatterSwung;     // ���� ����
        public bool IsBuntAttempt;   // ��Ʈ �õ� ����

        public bool Contact;         // ��Ʈ�� �¾Ҵ���
        public bool BallInPlayFair;  // ���÷��� ��� �������� ��������
        public bool BallInFoul;      // �Ŀ�����

        public bool IsFoulTipCaught; // �Ŀ� �� ���� ���
        public bool HitBatter;       // ���� �´� ��(HBP)
    }

    //public enum HitType
    //{
    //    Unknown,
    //    GroundBall,
    //    FlyBall,
    //    LineDrive,
    //    HomeRun,
    //}

    /// <summary>
    /// Ÿ�� ���. �� ����� ��Ÿ/�ƿ�/Ȩ�� ���� ������ �� ���.
    /// </summary>
    public struct HitBallResult
    {
        public HitType HitType;
        public bool IsFair;
        public bool IsCaughtInAir; // �ö��� �ƿ� ����
        public int BaseCount;     // 1~4 (Ȩ���� 4)
        public bool IsHomeRun;
    }


