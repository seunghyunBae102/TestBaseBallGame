
    /// <summary>
    /// 타자가(혹은 타자 대주자가) 몇 루까지 진루 권리를 얻었는지.
    /// 실제 강제 진루/득점 계산은 RunnerAdvanceModule이 한다.
    /// </summary>
    public struct BatterAwardedBase
    {
        /// <summary>
        /// 부여된 베이스 수 (1=1루타 or 볼넷, 2=2루타, 3=3루타, 4=홈런).
        /// </summary>
        public int BaseCount;

        public bool IsHit;        // 안타인지
        public bool IsWalk;       // 볼넷인지
        public bool IsHitByPitch; // 사구인지
        public bool IsError;      // 수비 실책으로 인한 진루인지(나중용)
    }
