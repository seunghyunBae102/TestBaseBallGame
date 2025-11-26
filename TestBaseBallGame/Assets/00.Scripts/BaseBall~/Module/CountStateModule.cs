// Scripts/Game/Baseball/Match/Modules/CountStateModule.cs
using UnityEngine;


    /// <summary>
    /// ��/��Ʈ/�ƿ� ī��Ʈ ���¸� �����ϰ� CountChanged �̺�Ʈ�� �����Ѵ�.
    /// </summary>
    public class CountStateModule : Module
    {
        [SerializeField] private GameModeConfig _modeConfig;

        public int Balls { get; private set; }
        public int Strikes { get; private set; }
        public int Outs { get; private set; }

        protected override void OnInit()
        {
            base.OnInit();
            ResetCount(outs: 0);
        }

        public void ResetCount(int outs)
        {
            Balls = 0;
            Strikes = 0;
            Outs = outs;
            PublishCountChanged();
        }

        public void AddBall()
        {
            Balls++;
            if (Balls > _modeConfig.maxBalls)
                Balls = _modeConfig.maxBalls;
            PublishCountChanged();
        }

        public void AddStrike()
        {
            Strikes++;
            if (Strikes > _modeConfig.maxStrikes)
                Strikes = _modeConfig.maxStrikes;
            PublishCountChanged();
        }

        public void AddOut()
        {
            Outs++;
            if (Outs > _modeConfig.maxOuts)
                Outs = _modeConfig.maxOuts;
            PublishCountChanged();
        }

        private void PublishCountChanged()
        {
            Events?.Publish(new CountChanged
            {
                Balls = Balls,
                Strikes = Strikes,
                Outs = Outs
            });
        }
    }


