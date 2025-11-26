// Scripts/Game/Baseball/Rules/Rule_HitResultModule.cs


    /// <summary>
    /// HitBallResult(Ÿ�� ���) �� ��Ÿ ����/Ȩ��/�ö��� �ƿ� ó��.
    /// �ַ�/������ RunnerManager, ScoreStateModule�� ���.
    /// </summary>
    public class Rule_HitResultModule : Module
    {
        [UnityEngine.SerializeField] private GameModeConfig _modeConfig;
        [UnityEngine.SerializeField] private CountStateModule _countState;

        protected override void OnInit()
        {
            base.OnInit();
            Events?.Subscribe<HitBallResult>(OnHitBallResult);
        }

        protected override void OnShutdown()
        {
            Events?.Unsubscribe<HitBallResult>(OnHitBallResult);
            base.OnShutdown();
        }

        private void OnHitBallResult(HitBallResult r)
        {
            // �Ŀ��̸� ��Ʈ����ũ or ����
            if (!r.IsFair)
            {
                HandleFoul(r);
                return;
            }

            // �ö��� ���� �� Ÿ�� �ƿ� + ���÷��� ����
            if (r.IsCaughtInAir)
            {
                HandleFlyOut(r);
                return;
            }

            // Ȩ��
            if (r.IsHomeRun)
            {
                HandleHomeRun(r);
                return;
            }

            // �Ϲ� ��Ÿ (��Ÿ/2��Ÿ/3��Ÿ/�λ��̵� Ȩ��)
            HandleHit(r);
        }

        private void HandleFoul(HitBallResult r)
        {
            // 2��Ʈ ���� �Ŀ��� ��Ʈ ���� X
            if (_countState.Strikes < _modeConfig.maxStrikes - 1)
            {
                _countState.AddStrike();
            }

            // ���÷��̴� �ƴϹǷ� AtBat ����
        }

        private void HandleFlyOut(HitBallResult r)
        {
            _countState.AddOut();
            _countState.ResetCount(_countState.Outs);

            Events?.Publish(new BatterOut
            {
                Reason = "FlyOut"
            });

            Events?.Publish(new AtBatEnded
            {
                PutBallInPlay = true,
                Walk = false,
                StrikeOut = false,
                HitByPitch = false
            });
        }

        private void HandleHomeRun(HitBallResult r)
        {
            // Ȩ���� Ÿ�� + ���̽� �� ��� ���ڰ� ����
            // �� ���� ���� RunnerManager�� ����ؼ� RunsScored �̺�Ʈ�� ��� ������ �δ� �� ���������,
            // ���⼭�� �ּ��� Ÿ�� ���� 1���� ����.
            Events?.Publish(new BatterAwardedBase
            {
                BaseCount = 4,
                IsHit = true,
                IsWalk = false,
                IsHitByPitch = false,
                IsError = false
            });

            _countState.ResetCount(_countState.Outs);

            Events?.Publish(new AtBatEnded
            {
                PutBallInPlay = true,
                Walk = false,
                StrikeOut = false,
                HitByPitch = false
            });
        }

        private void HandleHit(HitBallResult r)
        {
            int baseCount = UnityEngine.Mathf.Clamp(r.BaseCount, 1, 4);

            Events?.Publish(new BatterAwardedBase
            {
                BaseCount = baseCount,
                IsHit = true,
                IsWalk = false,
                IsHitByPitch = false,
                IsError = false
            });

            // Ÿ�ڸ� ���� ���δ� RunnerManager�� ���ڵ�� ���� ����.
            _countState.ResetCount(_countState.Outs);

            Events?.Publish(new AtBatEnded
            {
                PutBallInPlay = true,
                Walk = false,
                StrikeOut = false,
                HitByPitch = false
            });
        }
    }


