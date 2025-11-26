// Scripts/Game/Baseball/Match/Runner/RunnerTypes.cs


    /// <summary>
    /// �� ���ڸ� �����ϴ� Id. (team + Ÿ�� index + uniqueIndex)
    /// </summary>
    public struct RunnerId
    {
        public TeamSide team;
        public int batterIndex; // Ÿ�� index
        public int uniqueIndex; // ���ÿ� ���� ���ڰ� ���� �� �����Ƿ�
    }

    public enum RunnerPhase
    {
        OnBench,
        AtBat,
        OnBase,
        Out,
        Scored
    }

    public struct RunnerState
    {
        public RunnerId id;
        public int baseIndex; // 0: Ȩ, 1: 1��, 2: 2��, 3: 3��
        public RunnerPhase phase;

        public bool IsOnBase => phase == RunnerPhase.OnBase;
    }


