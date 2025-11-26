// Scripts/Game/Baseball/Match/BaseballGameManager.cs
using UnityEngine;


    /// <summary>
    /// �߱� ��� ��ü�� �Ѱ��ϴ� �Ŵ���.
    /// - ���� ����/����
    /// - �̴� ����/���� �Ǵ�
    /// - extra innings ����
    /// �� å������.
    /// </summary>
    public class BaseballGameManager : ManagerBase
    {
        [SerializeField] private GameModeConfig _modeConfig;

        [Header("State Modules")]
        [SerializeField] private Match.Modules.CountStateModule _countState;
        [SerializeField] private Match.Modules.InningStateModule _inningState;
        [SerializeField] private Match.Modules.ScoreStateModule _scoreState;

        private bool _gameEnded;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        protected override void Initialize()
        {
            base.Initialize();

            // �̺�Ʈ ����
            Events?.Subscribe<CountChanged>(OnCountChanged);
            Events?.Subscribe<InningHalfEnded>(OnInningEnded);
            Events?.Subscribe<GameStarted>(OnGameStarted);
        }

        protected override void OnDestroy()
        {
            // ���� ����
            Events?.Unsubscribe<CountChanged>(OnCountChanged);
            Events?.Unsubscribe<InningHalfEnded>(OnInningEnded);
            Events?.Unsubscribe<GameStarted>(OnGameStarted);

            base.OnDestroy();
        }

        private void Start()
        {
            // �׽�Ʈ��: �ڵ����� GameStarted ������
            Events?.Publish(new GameStarted());
        }

        private void OnGameStarted(GameStarted _)
        {
            _gameEnded = false;

            // �̴�/ī��Ʈ �ʱ�ȭ�� �� StateModule�� OnInit���� ó����.
            Debug.Log("BaseballGameManager: Game Started");
        }

        private void OnCountChanged(CountChanged e)
        {
            // ���⼭�� ī��Ʈ ��ȭ�� ���� "�̴� ����"�� �Ű澴��.
            if (e.Outs >= _modeConfig.maxOuts)
            {
                // �� �̴� ����
                Events?.Publish(new InningHalfEnded
                {
                    InningNumber = _inningState.CurrentInning,
                    Half = _inningState.CurrentHalf
                });

                // ī��Ʈ Reset�� �ܺο��� ���� ȣ���ϰų�
                // ���⼭ CountStateModule.ResetCount�� ȣ���ص� ��.
            }
        }

        private void OnInningEnded(InningHalfEnded e)
        {
            // InningStateModule���� ���� ���� ����/�̴� ���� ��Ű��
            _inningState.EndHalfInning();

            // ���� �̴� �������� üũ
            if (e.InningNumber >= _modeConfig.regularInnings && _inningState.CurrentHalf == InningHalf.Top)
            {
                TryEndGameIfNoExtra();
            }
        }

        private void TryEndGameIfNoExtra()
        {
            if (!_modeConfig.allowExtraInnings)
            {
                EndGameByScore();
            }
            else
            {
                // �������̸�, ������ ���Ƶ� ��� ����
                // (�߰� ������ �ְ� ������ ���⿡)
            }
        }

        private void EndGameByScore()
        {
            if (_gameEnded) return;
            _gameEnded = true;

            TeamSide winner;
            bool isDraw = false;

            if (_scoreState.HomeScore > _scoreState.AwayScore)
                winner = TeamSide.Home;
            else if (_scoreState.HomeScore < _scoreState.AwayScore)
                winner = TeamSide.Away;
            else
            {
                // ���º� ��� ���δ� ��� ������ ���߿� �־ ��.
                isDraw = true;
                winner = TeamSide.Home; // �ǹ� ����
            }

            Events?.Publish(new GameEnded
            {
                Winner = winner,
                IsDraw = isDraw
            });

            Debug.Log($"Game Ended. Home: {_scoreState.HomeScore}, Away: {_scoreState.AwayScore}");
        }
    }


