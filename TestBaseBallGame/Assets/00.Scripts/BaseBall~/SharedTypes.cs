// Scripts/Game/Baseball/Shared/BaseballSharedTypes.cs

    public enum TeamSide
    {
        Home = 0,
        Away = 1,
    }

    public enum InningHalf
    {
        Top,
        Bottom,
    }

    public enum PitchOutcomeType
    {
        None,
        CalledStrike,    // 스트존 통과, 스윙 X
        SwingingStrike,  // 스윙 헛치기
        Foul,            // 일반 파울 (2스트 전까진 스트로 카운트)
        FoulBunt,        // 번트 파울 (2스트에서도 삼진 처리 대상)
        Ball,            // 스트존 밖 + 스윙 X
        HitByPitch,      // 몸에 맞는 볼 (HBP)
        InPlay           // 인플레이 (타구 발생) – 이후 HitResult에서 처리
    }

    public enum HitType
    {
        None,
        GroundBall,
        FlyBall,
        LineDrive,
        HomeRun
    }



