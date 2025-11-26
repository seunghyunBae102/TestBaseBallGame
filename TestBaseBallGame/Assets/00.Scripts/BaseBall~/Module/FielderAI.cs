// Scripts/Game/Baseball/Match/Fielder/FielderAI.cs

    /// <summary>
    /// �ſ� �ܼ��� ���� AI.
    /// ��� ���÷��� Ÿ���� �߻��ϸ�, ���� ���� ����� ����� �ϳ��� ���
    /// "�������� ��������"�� ����� ������ ������ ����.
    /// </summary>
    public class FielderAI : Module
    {
        [SerializeField] private FielderManager _fielderManager;
        [SerializeField] private Transform _currentBallTransform;
        [SerializeField] private TeamSide _defenseTeamSide = TeamSide.Home;

        // ���߿� FielderMovementModule�� �и��ϸ�,
        // ���⼭�� "Ÿ�� ��ġ"�� ������, �̵��� Movement�� �ð� �� �� ����.
        [SerializeField] private float _moveSpeed = 8f;

        private FielderActor _currentResponder;

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

        protected override void Tick(float dt)
        {
            base.Tick(dt);

            if (_currentResponder == null || _currentBallTransform == null) return;

            var t = _currentResponder.CachedTransform;
            Vector3 targetPos = _currentBallTransform.position;
            Vector3 dir = targetPos - t.position;
            dir.y = 0f; // ���� �̵���

            float dist = dir.magnitude;
            if (dist < 0.1f) return;

            dir /= dist;
            t.position += dir * _moveSpeed * dt;
        }

        private void OnHitBallResult(HitBallResult r)
        {
            if (!r.IsFair || r.IsCaughtInAir)
            {
                // ���� ���̰ų� �Ŀ��̸� ���� ���� �ʿ� ����
                _currentResponder = null;
                return;
            }

            if (_currentBallTransform == null || _fielderManager == null) return;

            // ���� ���� ����� ����� ����
            var ballPos = _currentBallTransform.position;
            var fielder = _fielderManager.GetClosestFielder(ballPos, _defenseTeamSide);

            _currentResponder = fielder;
        }

        public void SetCurrentBallTransform(Transform ballTr)
        {
            _currentBallTransform = ballTr;
        }

        public void SetDefenseTeam(TeamSide team)
        {
            _defenseTeamSide = team;
        }
    }


///// <summary>
///// ���� �ܼ��� �ʴ� AI ����:
///// - ���÷��� Ÿ���� ������, ���� �� �ʴ� �� ���� ����� �ʴ��� ���� �Ѿư�����.
///// </summary>
//public class FielderAI : Module
//{
//    [SerializeField] private FielderManager _fielderManager;
//    [SerializeField] private Transform _ballTransform;  // HitBallModule�� �� ��ġ
//    [SerializeField] private TeamSide _defenseTeam = TeamSide.Home;

//    private FielderActor _targetFielder;

//    protected override void OnInit()
//    {
//        base.OnInit();
//        Events?.Subscribe<HitBallResult>(OnHitBallResult);
//    }

//    protected override void Tick(float dt)
//    {
//        base.Tick(dt);

//        if (_targetFielder != null && _ballTransform != null)
//        {
//            _targetFielder.MoveTowards(_ballTransform.position, dt);
//        }
//    }

//    protected override void OnShutdown()
//    {
//        Events?.Unsubscribe<HitBallResult>(OnHitBallResult);
//        base.OnShutdown();
//    }

//    private void OnHitBallResult(HitBallResult r)
//    {
//        if (!r.IsFair) return;
//        if (_ballTransform == null || _fielderManager == null) return;

//        _targetFielder = _fielderManager.GetClosestFielder(_ballTransform.position, _defenseTeam);
//    }
//}//}
