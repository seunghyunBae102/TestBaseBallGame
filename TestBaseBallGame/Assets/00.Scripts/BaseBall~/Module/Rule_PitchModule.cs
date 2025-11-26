// Scripts/Game/Baseball/Rules/Rule_PitchModule.cs


    /// <summary>
    /// RawPitchResult(���� ����) �� ��/��Ʈ/����/�籸/��籸 ó��.
    /// CountStateModule�� GameModeConfig�� ����.
    /// </summary>
    public class Rule_PitchModule : Module
    {
        [UnityEngine.SerializeField] private GameModeConfig _modeConfig;
        [UnityEngine.SerializeField] private CountStateModule _countState;

        protected override void OnInit()
        {
            base.OnInit();
            Events?.Subscribe<RawPitchResult>(OnRawPitchResult);
        }

        protected override void OnShutdown()
        {
            Events?.Unsubscribe<RawPitchResult>(OnRawPitchResult);
            base.OnShutdown();
        }

        private void OnRawPitchResult(RawPitchResult e)
        {
            // ����(HBP)
            if (e.HitBatter)
            {
                HandleHitByPitch();
                return;
            }

            // ���÷��̷� ����Ǹ� ���� HitResult���� ó��.
            if (e.Contact && e.BallInPlayFair)
            {
                // ���⼭�� ī��Ʈ�� ����(�ƿ� ���� ���δ� HitResult�� ó��)
                _countState.ResetCount(_countState.Outs);
                return;
            }

            // ���� �� �߰� ��Ʈ����ũ �� ���̸� ��
            if (!e.BatterSwung && !e.IsInStrikeZone)
            {
                HandleBall();
                return;
            }

            // �����ߴµ� �꽺�� or �Ŀ��� ���� �� ��Ʈ����ũ
            if (e.BatterSwung && !e.Contact)
            {
                HandleStrike();
                return;
            }

            // ���� �� �ߴµ� ��Ʈ �� �ȿ� �� ��Ʈ����ũ(�� ����)
            if (!e.BatterSwung && e.IsInStrikeZone)
            {
                HandleStrike();
                return;
            }

            // �� ��(�Ŀ� ��)�� HitResult���� ó��
        }

        private void HandleBall()
        {
            _countState.AddBall();

            if (_countState.Balls >= _modeConfig.maxBalls)
            {
                // ����
                Events?.Publish(new BatterAwardedBase
                {
                    BaseCount = 1,
                    IsHit = false,
                    IsWalk = true,
                    IsHitByPitch = false,
                    IsError = false
                });

                _countState.ResetCount(_countState.Outs);
                Events?.Publish(new AtBatEnded
                {
                    // Offense/Index�� LineupManager�� ä����
                    PutBallInPlay = false,
                    Walk = true,
                    StrikeOut = false,
                    HitByPitch = false
                });
            }
        }

        private void HandleStrike()
        {
            _countState.AddStrike();

            if (_countState.Strikes >= _modeConfig.maxStrikes)
            {
                // ����
                _countState.AddOut();
                _countState.ResetCount(_countState.Outs);

                Events?.Publish(new BatterOut
                {
                    Reason = "StrikeOut"
                });

                Events?.Publish(new AtBatEnded
                {
                    PutBallInPlay = false,
                    Walk = false,
                    StrikeOut = true,
                    HitByPitch = false
                });
            }
        }

        private void HandleHitByPitch()
        {
            Events?.Publish(new BatterAwardedBase
            {
                BaseCount = 1,
                IsHit = false,
                IsWalk = false,
                IsHitByPitch = true,
                IsError = false
            });

            _countState.ResetCount(_countState.Outs);

            Events?.Publish(new AtBatEnded
            {
                PutBallInPlay = false,
                Walk = false,
                StrikeOut = false,
                HitByPitch = true
            });
        }
    }


