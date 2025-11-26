// Scripts/Game/Baseball/State/InningStateModule.cs


    /// <summary>
    /// �̴�/����/���ݡ����� �� ����.
    /// BaseballGameManager�� ���⸦ �����ϰų� �����Ѵ�.
    /// </summary>
    public class InningStateModule : Module
    {
        public int InningNumber { get; private set; } = 1;
        public bool IsTop { get; private set; } = true;
        public TeamSide Offense { get; private set; } = TeamSide.Away;
        public TeamSide Defense { get; private set; } = TeamSide.Home;

        protected override void OnInit()
        {
            base.OnInit();
            Events?.Subscribe<InningHalfStarted>(OnInningHalfStarted);
            Events?.Subscribe<InningHalfEnded>(OnInningHalfEnded);
        }

        protected override void OnShutdown()
        {
            Events?.Unsubscribe<InningHalfStarted>(OnInningHalfStarted);
            Events?.Unsubscribe<InningHalfEnded>(OnInningHalfEnded);
            base.OnShutdown();
        }

        private void OnInningHalfStarted(InningHalfStarted e)
        {
            InningNumber = e.InningNumber;
            IsTop = e.IsTop;
            Offense = e.Offense;
            Defense = e.Defense;
        }

        private void OnInningHalfEnded(InningHalfEnded e)
        {
            // �ʿ��ϸ� ���⼭ ���¸� ����ϰų� �α׸� ���� �� ����.
        }

        /// <summary>
        /// ��/�� �̴� ��ȯ�� ���� �����ϰ� ���� �� ȣ��.
        /// ������ BaseballGameManager���� ȣ��.
        /// </summary>
        public void AdvanceHalfInning()
        {
            if (IsTop)
            {
                // �� �� ��
                IsTop = false;
                Offense = TeamSide.Home;
                Defense = TeamSide.Away;
            }
            else
            {
                // �� �� ���� �̴� ��
                IsTop = true;
                InningNumber++;
                Offense = TeamSide.Away;
                Defense = TeamSide.Home;
            }

            Events?.Publish(new InningHalfStarted
            {
                InningNumber = InningNumber,
                IsTop = IsTop,
                Offense = Offense,
                Defense = Defense
            });
        }
    }


