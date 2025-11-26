// Scripts/Game/Baseball/Match/Modules/ScoreStateModule.cs
using UnityEngine;


    /// <summary>
    /// Ȩ/���� ������ �����ϰ� ScoreChanged �̺�Ʈ�� ����.
    /// RunsScored �̺�Ʈ�� �����ؼ� ������ �ø���.
    /// </summary>
    public class ScoreStateModule : Module
    {
        public int HomeScore { get; private set; }
        public int AwayScore { get; private set; }

        protected override void OnInit()
        {
            base.OnInit();
            Events?.Subscribe<RunsScored>(OnRunsScored);
        }

        protected override void OnShutdown()
        {
            Events?.Unsubscribe<RunsScored>(OnRunsScored);
            base.OnShutdown();
        }

        private void OnRunsScored(RunsScored e)
        {
            if (e.Team == TeamSide.Home) HomeScore += e.RunCount;
            else AwayScore += e.RunCount;

            Events?.Publish(new ScoreChanged
            {
                HomeScore = HomeScore,
                AwayScore = AwayScore
            });
        }
    }


