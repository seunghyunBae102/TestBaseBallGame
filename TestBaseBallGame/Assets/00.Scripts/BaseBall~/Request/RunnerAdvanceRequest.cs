// Scripts/Game/Baseball/Events/RunnerEvents.cs
    /// <summary>
    /// RunnerAI나 입력에서 "추가 진루"를 요청하는 이벤트.
    /// </summary>
    public struct RunnerAdvanceRequest
    {
        public RunnerId runnerId;
        public int extraBaseCount; // 1=한 베이스 더, 2=두 베이스 더 시도 등
    }
