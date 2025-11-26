// Scripts/Game/Baseball/Match/Ball/PitchBallModule.cs
using System;
using UnityEngine;


    /// <summary>
    /// ���� �� ���.
    /// - BeginPitchToStrikeZone(2D ��ǥ)�� ��Ʈ�� �� Ư�� ������ ���� ������.
    /// - ���� BeginPitch(startPos, v0)�� �״�� ��� ����.
    /// </summary>
    public class PitchBallModule : Module
    {
        [Header("Ball Behaviour")]
        [SerializeField] private BallBehaviourSO _baseBehaviour;
        [SerializeField] private BallSkillSO[] _skills;

        [Header("Visual")]
        [SerializeField] private Transform _ballVisual;
        [SerializeField] private float _ballRadius = 0.035f;

        [Header("Strike Zone & Colliders")]
        [SerializeField] private Collider _strikeZoneCollider;   // ���� ��Ʈ�� �ݶ��̴�
        [SerializeField] private Collider _batterBodyCollider;
        [SerializeField] private Collider _batCollider;
        [SerializeField] private Collider _catcherGloveCollider;

        [Header("2D Targeting")]
        [SerializeField] private Transform _releasePoint;      // ���� �տ��� ������ ��ġ
        [SerializeField] private Transform _strikeZoneOrigin;  // ��Ʈ�� �߾� ���� Transform
        [SerializeField] private Vector2 _strikeZoneSize = new Vector2(0.5f, 0.7f); // ��, ���� (���� ����)
        [SerializeField] private float _defaultFlightTime = 0.5f;   // �⺻ ���� ����ð�

        [Header("Environment")]
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private LayerMask _environmentMask;
        [SerializeField] private float _maxFlightTime = 5f;

        /// <summary>
        /// ��Ʈ�� �¾��� ��: (���� ��ġ, �� �ӵ�)
        /// </summary>
        public event Action<Vector3, Vector3> OnBatContact;

        private BallState _state;
        private BallContext _ctx;
        private bool _isActive;
        private bool _batterSwung;
        private bool _isBunt;
        private float _elapsed;

        protected override void OnInit()
        {
            base.OnInit();
            _isActive = false;
        }

        protected override void Tick(float dt)
        {
            base.Tick(dt);
            if (!_isActive || _baseBehaviour == null)
                return;

            _elapsed += dt;
            if (_elapsed > _maxFlightTime)
            {
                PublishResultNoContact(false);
                Deactivate();
                return;
            }

            Vector3 prevPos = _state.position;

            // Behaviour
            _baseBehaviour.SimulateStep(ref _state, in _ctx, dt);

            // Skills
            if (_skills != null)
            {
                for (int i = 0; i < _skills.Length; i++)
                    _skills[i]?.Modify(ref _state, in _ctx, dt);
            }

            if (_ballVisual != null)
                _ballVisual.position = _state.position;

            Vector3 delta = _state.position - prevPos;
            float dist = delta.magnitude;
            if (dist <= 0f) return;

            Vector3 dir = delta / dist;
            RaycastHit hit;

            // bat/body/glove ���� �ݶ��̴� �񱳷�, ����/ȯ���� ���̾�� ó��
            if (Physics.SphereCast(prevPos, _ballRadius, dir, out hit, dist, ~0, QueryTriggerInteraction.Collide))
            {
                HandleCollision(hit);
            }
            else
            {
                if (Physics.Raycast(prevPos, dir, out hit, dist, _groundMask | _environmentMask, QueryTriggerInteraction.Ignore))
                {
                    HandleGroundOrEnv(hit);
                }
            }
        }

        // �������������������������� �ܺ� API ��������������������������

        /// <summary>
        /// ����ó�� "���� ��ġ, �ʱ� �ӵ�"�� �ٷ� ������ ����.
        /// </summary>
        public void BeginPitch(Vector3 startPos, Vector3 initialVelocity, BallContext ctxOverride = default)
        {
            _state = new BallState
            {
                position = startPos,
                velocity = initialVelocity,
                isAlive = true
            };

            _ctx = ctxOverride;
            if (_ctx.gravity == 0f)
                _ctx.gravity = Physics.gravity.y;
            _ctx.targetPos = Vector3.zero;

            _batterSwung = false;
            _isBunt = false;
            _elapsed = 0f;
            _isActive = true;

            if (_ballVisual != null)
                _ballVisual.position = startPos;

            _baseBehaviour?.Initialize(ref _state, in _ctx);
        }

        /// <summary>
        /// 2D ��Ʈ�� ��ǥ(0~1,0~1)�� �����ؼ� �� �������� ������ ����.
        /// zonePoint01.x : ��(0) �� ��(1)
        /// zonePoint01.y : �Ʒ�(0) �� ��(1)
        /// </summary>
        public void BeginPitchToStrikeZone(Vector2 zonePoint01, float? overrideFlightTime = null)
        {
            if (_releasePoint == null || _strikeZoneOrigin == null)
            {
                Debug.LogWarning("PitchBallModule: ReleasePoint �Ǵ� StrikeZoneOrigin ������");
                return;
            }

            float flightTime = overrideFlightTime ?? _defaultFlightTime;
            flightTime = Mathf.Max(0.1f, flightTime);

            Vector3 startPos = _releasePoint.position;
            Vector3 worldTarget = ComputeStrikeZoneWorldPoint(zonePoint01);

            Vector3 v0 = ComputeBallisticVelocity(startPos, worldTarget, flightTime, Physics.gravity.y);

            BallContext ctx = new BallContext
            {
                gravity = Physics.gravity.y,
                targetPos = worldTarget
            };

            BeginPitch(startPos, v0, ctx);
        }

        /// <summary>
        /// Ÿ�ڰ� ���������� �˸� (BatSwingModule���� ȣ��).
        /// </summary>
        public void NotifyBatterSwing(bool isBunt)
        {
            _batterSwung = true;
            _isBunt = isBunt;
        }

        // �������������������������� ���� ���� ��������������������������

        private Vector3 ComputeStrikeZoneWorldPoint(Vector2 zonePoint01)
        {
            // zonePoint01 = (0,0)~(1,1)
            float x = Mathf.Clamp01(zonePoint01.x) - 0.5f; // -0.5~0.5
            float y = Mathf.Clamp01(zonePoint01.y) - 0.5f;

            Vector3 right = _strikeZoneOrigin.right;
            Vector3 up = _strikeZoneOrigin.up;
            Vector3 center = _strikeZoneOrigin.position;

            Vector3 offset =
                x * _strikeZoneSize.x * right +
                y * _strikeZoneSize.y * up;

            return center + offset;
        }

        /// <summary>
        /// ������ � �������� Ư�� �ð� �ȿ� target�� ����ϴ� �ʱ� �ӵ� ���.
        /// </summary>
        private Vector3 ComputeBallisticVelocity(Vector3 start, Vector3 target, float time, float gravityY)
        {
            // �� = target - start
            Vector3 delta = target - start;

            float vx = delta.x / time;
            float vz = delta.z / time;

            // y(t) = y0 + vy*t + 0.5*g*t^2 �� vy = (��y - 0.5*g*t^2)/t
            float vy = (delta.y - 0.5f * gravityY * time * time) / time;

            return new Vector3(vx, vy, vz);
        }

        private void HandleCollision(RaycastHit hit)
        {
            if (hit.collider == null) return;

            // ��Ʈ
            if (hit.collider == _batCollider)
            {
                OnBatContact?.Invoke(hit.point, _state.velocity);
                Deactivate();
                return;
            }

            // Ÿ�� ��
            if (hit.collider == _batterBodyCollider)
            {
                var result = new RawPitchResult
                {
                    HitBatter = true,
                    BatterSwung = _batterSwung,
                    IsInStrikeZone = false,
                    Contact = false,
                    BallInPlayFair = false,
                    BallInFoul = false,
                    IsFoulTipCaught = false
                };
                Events?.Publish(result);
                Deactivate();
                return;
            }

            // ���� ��Ʈ
            if (hit.collider == _catcherGloveCollider)
            {
                bool inZone = _strikeZoneCollider != null &&
                              _strikeZoneCollider.bounds.Contains(hit.point);

                PublishResultNoContact(inZone);
                Deactivate();
                return;
            }

            // ��Ÿ �ݶ��̴��� ȯ�� ���
            HandleGroundOrEnv(hit);
        }

        private void HandleGroundOrEnv(RaycastHit hit)
        {
            // �׳� �� ó��
            var result = new RawPitchResult
            {
                HitBatter = false,
                BatterSwung = _batterSwung,
                IsInStrikeZone = false,
                Contact = false,
                BallInPlayFair = false,
                BallInFoul = false,
                IsFoulTipCaught = false
            };
            Events?.Publish(result);
            Deactivate();
        }

        private void PublishResultNoContact(bool isInZone)
        {
            var result = new RawPitchResult
            {
                IsInStrikeZone = isInZone,
                BatterSwung = _batterSwung,
                IsBuntAttempt = _isBunt,
                Contact = false,
                BallInPlayFair = false,
                BallInFoul = false,
                IsFoulTipCaught = false,
                HitBatter = false
            };
            Events?.Publish(result);
        }

        private void Deactivate()
        {
            _isActive = false;
            _state.isAlive = false;
        }
    }


