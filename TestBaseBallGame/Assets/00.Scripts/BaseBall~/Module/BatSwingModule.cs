// Scripts/Game/Baseball/Match/Batting/BatSwingModule.cs
using UnityEngine;


    /// <summary>
    /// Ÿ�� ����/��Ʈ ����/������ �������,
    /// ��Ʈ�� ���� ����� �� HitBallModule�� Ÿ���� �ѱ�� ���.
    /// </summary>
    public class BatSwingModule : Module
    {
        [Header("References")]
        [SerializeField] private PitchBallModule _pitchBallModule;
        [SerializeField] private HitBallModule _hitBallModule;

        [Header("Behaviours")]
        [SerializeField] private BallBehaviourSO _flyBehaviour;
        [SerializeField] private BallBehaviourSO _groundBehaviour;

        [Header("Swing Settings")]
        [SerializeField] private float _baseExitVelocity = 35f;
        [SerializeField] private float _launchAngleMin = 5f;
        [SerializeField] private float _launchAngleMax = 35f;
        [SerializeField] private float _foulAngleThreshold = 25f;

        private PlayerDataSO _currentBatter;
        private PlayerDataSO _currentPitcher;
        private TeamSide _offenseTeam;

        private bool _pendingSwing;
        private bool _pendingBunt;
        private Vector3 _swingDirection = Vector3.forward;

        protected override void OnInit()
        {
            base.OnInit();

            Events?.Subscribe<AtBatStarted>(OnAtBatStarted);

            if (_pitchBallModule != null)
                _pitchBallModule.OnBatContact += OnBatContact;
        }

        protected override void OnShutdown()
        {
            Events?.Unsubscribe<AtBatStarted>(OnAtBatStarted);

            if (_pitchBallModule != null)
                _pitchBallModule.OnBatContact -= OnBatContact;

            base.OnShutdown();
        }

        private void OnAtBatStarted(AtBatStarted e)
        {
            _currentBatter = e.Batter;
            _currentPitcher = e.Pitcher;
            _offenseTeam = e.Offense;

            _pendingSwing = false;
            _pendingBunt = false;
            _swingDirection = Vector3.forward;
        }

        /// <summary>
        /// �÷��̾�/AI�� ���� �õ�. swingDir: Ÿ�� ���� ���� ����, isBunt: ��Ʈ ����.
        /// </summary>
        public void RequestSwing(Vector3 swingDir, bool isBunt)
        {
            _pendingSwing = true;
            _pendingBunt = isBunt;

            if (swingDir.sqrMagnitude > 0.0001f)
                _swingDirection = swingDir.normalized;

            _pitchBallModule?.NotifyBatterSwing(isBunt);
        }

        private void OnBatContact(Vector3 contactPos, Vector3 incomingVelocity)
        {
            // Ÿ�� ����
            float contact = 50f;
            float power = 50f;

            if (_currentBatter != null)
            {
                contact = _currentBatter.stats.batting.contact;
                power = _currentBatter.stats.batting.power;
            }

            float exitVel = ComputeExitVelocity(power, incomingVelocity.magnitude);

            HitType hitType;
            Vector3 launchDir = ComputeLaunchDirection(contactPos, incomingVelocity, _swingDirection, contact, out hitType);

            bool isFoul = IsFoulDirection(launchDir);

            if (_pendingBunt)
            {
                hitType = HitType.GroundBall;
                launchDir = new Vector3(launchDir.x, 0.1f, launchDir.z).normalized;
            }

            Vector3 launchVelocity = launchDir * exitVel;

            BallBehaviourSO behaviour = hitType == HitType.GroundBall ? _groundBehaviour : _flyBehaviour;

            if (_hitBallModule != null && behaviour != null)
            {
                _hitBallModule.BeginHit(contactPos, launchVelocity, hitType, behaviour, null);
            }

            _pendingSwing = false;
            _pendingBunt = false;
        }

        private float ComputeExitVelocity(float powerStat, float incomingSpeed)
        {
            float powerFactor = 0.8f + (powerStat / 100f) * 0.7f;
            float baseFactor = 0.3f + (powerStat / 100f) * 0.5f;

            float exitVel = _baseExitVelocity * powerFactor + incomingSpeed * baseFactor;
            exitVel = Mathf.Clamp(exitVel, 20f, 60f);
            return exitVel;
        }

        private Vector3 ComputeLaunchDirection(
            Vector3 contactPos,
            Vector3 incomingVel,
            Vector3 swingDir,
            float contactStat,
            out HitType hitType)
        {
            Vector3 forward = swingDir.sqrMagnitude > 0.0001f ? swingDir : Vector3.forward;

            float contact01 = Mathf.Clamp01(contactStat / 100f);
            float angleMin = Mathf.Lerp(0f, _launchAngleMin, contact01);
            float angleMax = Mathf.Lerp(10f, _launchAngleMax, contact01);
            float angle = Random.Range(angleMin, angleMax);

            float horizJitter = Mathf.Lerp(20f, 5f, contact01);
            float horizAngle = Random.Range(-horizJitter, horizJitter);

            Quaternion horizRot = Quaternion.AngleAxis(horizAngle, Vector3.up);
            Vector3 dir = horizRot * forward;

            Quaternion vertRot = Quaternion.AngleAxis(angle, Vector3.Cross(Vector3.up, dir));
            dir = vertRot * dir;
            dir.Normalize();

            if (angle < 10f)
                hitType = HitType.GroundBall;
            else if (angle < 20f)
                hitType = HitType.LineDrive;
            else if (angle < 35f)
                hitType = HitType.FlyBall;
            else
                hitType = HitType.FlyBall;

            return dir;
        }

        private bool IsFoulDirection(Vector3 launchDir)
        {
            Vector3 proj = new Vector3(launchDir.x, 0f, launchDir.z).normalized;
            if (proj.sqrMagnitude < 0.0001f)
                return false;

            float angleFromCenter = Vector3.SignedAngle(Vector3.forward, proj, Vector3.up);
            return Mathf.Abs(angleFromCenter) > _foulAngleThreshold;
        }
    }


