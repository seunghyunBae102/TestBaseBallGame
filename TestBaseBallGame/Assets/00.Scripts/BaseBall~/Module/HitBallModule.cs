using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 타구를 pre-sim(전체 궤적 시뮬) 후 재생하는 모듈.
/// Rebuild를 통해 스킬을 끼우고 궤적을 다시 계산할 수 있다.
/// </summary>
public class HitBallModule : Module
{
    [Header("Visual")]
    [SerializeField] private Transform _ballVisual;
    [SerializeField] private float _ballRadius = 0.035f;

    [Header("Default Behaviours")]
    [SerializeField] private BallBehaviourSO _defaultFlyBehaviour;
    [SerializeField] private BallBehaviourSO _defaultGroundBehaviour;

    [Header("Default Skills")]
    [SerializeField] private BallSkillSO[] _defaultSkills;

    [Header("Environment")]
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private LayerMask _fenceMask;
    [SerializeField] private float _homeRunFenceZ = 110f; // 홈런 기준 z
    [SerializeField] private float _maxFlightTime = 8f;

    [Header("Fair Zone")]
    [SerializeField] private float _foulLineAngle = 45f; // 홈 기준 ±각도

    [Header("Simulation")]
    [SerializeField] private float _simDeltaTime = 1f / 120f;
    [SerializeField] private float _playbackSpeed = 1f;

    // ───────────── Path 데이터 ─────────────
    [System.Serializable]
    public struct BallPathPoint
    {
        public float time;
        public Vector3 position;
        public Vector3 velocity;

        public BallPathPoint(float time, Vector3 position, Vector3 velocity)
        {
            this.time = time;
            this.position = position;
            this.velocity = velocity;
        }
    }

    private List<BallPathPoint> _path = new List<BallPathPoint>();
    private float _pathDuration;
    private HitBallResult _cachedResult;
    private bool _hasResult;

    private bool _isPlaying;
    private float _playbackTime;

    // ───────────── 초기 조건 캐시 (Rebuild용) ─────────────
    private Vector3 _initialPos;
    private Vector3 _initialVelocity;
    private HitType _initialHitType;
    private BallBehaviourSO _initialBehaviour;
    private BallSkillSO[] _initialSkills;

    protected override void OnInit()
    {
        base.OnInit();
        _isPlaying = false;
        _hasResult = false;
        _path.Clear();
    }

    protected override void Tick(float dt)
    {
        base.Tick(dt);

        if (!_isPlaying || _path.Count == 0)
            return;

        _playbackTime += dt * _playbackSpeed;

        if (_playbackTime >= _pathDuration)
        {
            // 마지막 지점으로 스냅
            var last = _path[_path.Count - 1];
            if (_ballVisual != null)
                _ballVisual.position = last.position;

            _isPlaying = false;

            if (_hasResult)
                Events?.Publish(_cachedResult);

            return;
        }

        int index = Mathf.Clamp(Mathf.FloorToInt(_playbackTime / _simDeltaTime), 0, _path.Count - 1);
        var p = _path[index];

        if (_ballVisual != null)
            _ballVisual.position = p.position;
    }

    /// <summary>
    /// BatSwingModule에서 호출하는 타구 시작.
    /// 여기서 초기 조건을 기록하고 전체 궤적을 pre-sim한다.
    /// </summary>
    public void BeginHit(
        Vector3 contactPos,
        Vector3 launchVelocity,
        HitType hitType,
        BallBehaviourSO behaviourOverride,
        BallSkillSO[] skillsOverride)
    {
        _initialPos = contactPos;
        _initialVelocity = launchVelocity;
        _initialHitType = hitType;

        BallBehaviourSO baseBehaviour =
            behaviourOverride ??
            (hitType == HitType.GroundBall ? _defaultGroundBehaviour : _defaultFlyBehaviour);

        _initialBehaviour = baseBehaviour;

        _initialSkills = skillsOverride ?? _defaultSkills;

        RebuildPathInternal(_initialBehaviour, _initialSkills);
    }

    /// <summary>
    /// 기존 궤적에 스킬을 추가해서 다시 시뮬레이션.
    /// (예: 맞은 이후 궤도를 꺾는 스킬)
    /// </summary>
    public void RebuildPathWithExtraSkill(BallSkillSO extraSkill)
    {
        if (extraSkill == null) return;

        var list = new List<BallSkillSO>();
        if (_initialSkills != null)
            list.AddRange(_initialSkills);
        list.Add(extraSkill);

        RebuildPathInternal(_initialBehaviour, list.ToArray());
    }

    /// <summary>
    /// 스킬 세트를 통째로 갈아끼우고 다시 시뮬레이션.
    /// </summary>
    public void RebuildPathOverrideSkills(BallSkillSO[] newSkills)
    {
        RebuildPathInternal(_initialBehaviour, newSkills);
    }

    // ───────────── 내부 pre-sim 로직 ─────────────
    private void RebuildPathInternal(BallBehaviourSO behaviour, BallSkillSO[] skills)
    {
        _path.Clear();
        _hasResult = false;
        _pathDuration = 0f;

        SimulateFullPath(
            _initialPos,
            _initialVelocity,
            _initialHitType,
            behaviour,
            skills,
            ref _path,
            out _pathDuration,
            out _cachedResult,
            out _hasResult
        );

        if (_path.Count > 0)
        {
            _isPlaying = true;
            _playbackTime = 0f;

            if (_ballVisual != null)
                _ballVisual.position = _path[0].position;
        }
        else
        {
            _isPlaying = false;
            if (_hasResult)
                Events?.Publish(_cachedResult);
        }
    }

    /// <summary>
    /// 전체 궤적을 Δt 단위로 시뮬레이션하고 path + result를 채운다.
    /// </summary>
    private void SimulateFullPath(
        Vector3 startPos,
        Vector3 launchVelocity,
        HitType hitType,
        BallBehaviourSO behaviour,
        BallSkillSO[] skills,
        ref List<BallPathPoint> path,
        out float pathDuration,
        out HitBallResult result,
        out bool hasResult)
    {
        path.Clear();
        result = default;
        hasResult = false;

        BallState state = new BallState
        {
            position = startPos,
            velocity = launchVelocity,
            isAlive = true
        };

        BallContext ctx = new BallContext
        {
            gravity = Physics.gravity.y,
            targetPos = Vector3.zero
        };

        behaviour?.Initialize(ref state, in ctx);

        float t = 0f;
        path.Add(new BallPathPoint(t, state.position, state.velocity));

        bool terminated = false;

        while (t < _maxFlightTime)
        {
            float dt = _simDeltaTime;
            t += dt;

            Vector3 prevPos = state.position;

            if (behaviour != null)
            {
                behaviour.SimulateStep(ref state, in ctx, dt);
            }
            else
            {
                state.velocity += new Vector3(0f, ctx.gravity * dt, 0f);
                state.position += state.velocity * dt;
            }

            if (skills != null)
            {
                for (int i = 0; i < skills.Length; i++)
                {
                    skills[i]?.Modify(ref state, in ctx, dt);
                }
            }

            Vector3 delta = state.position - prevPos;
            float dist = delta.magnitude;

            if (dist > 0f)
            {
                Vector3 dir = delta / dist;
                RaycastHit hit;

                // 펜스
                if (Physics.SphereCast(prevPos, _ballRadius, dir, out hit, dist, _fenceMask, QueryTriggerInteraction.Ignore))
                {
                    state.position = hit.point;
                    path.Add(new BallPathPoint(t, state.position, state.velocity));

                    result = BuildFenceHitResult(state.position, hitType);
                    hasResult = true;
                    terminated = true;
                    break;
                }

                // 지면
                if (Physics.SphereCast(prevPos, _ballRadius, dir, out hit, dist, _groundMask, QueryTriggerInteraction.Ignore))
                {
                    state.position = hit.point;
                    state.velocity = Vector3.zero;

                    path.Add(new BallPathPoint(t, state.position, state.velocity));

                    result = BuildGroundHitResult(state.position, hitType);
                    hasResult = true;
                    terminated = true;
                    break;
                }
            }

            path.Add(new BallPathPoint(t, state.position, state.velocity));
        }

        pathDuration = (path.Count - 1) * _simDeltaTime;

        if (!terminated)
        {
            result = BuildNoCollisionResult(state.position, hitType);
            hasResult = true;
        }
    }

    private HitBallResult BuildFenceHitResult(Vector3 hitPoint, HitType hitType)
    {
        bool isFair = IsFairDirection(hitPoint - Vector3.zero);
        bool isHomeRun = isFair && hitPoint.z >= _homeRunFenceZ;

        return new HitBallResult
        {
            HitType = isHomeRun ? HitType.HomeRun : hitType,
            IsFair = isFair,
            IsCaughtInAir = false,
            BaseCount = isHomeRun ? 4 : EstimateBaseCountByDistance(hitPoint),
            IsHomeRun = isHomeRun
        };
    }

    private HitBallResult BuildGroundHitResult(Vector3 hitPoint, HitType hitType)
    {
        bool isFair = IsFairDirection(hitPoint - Vector3.zero);

        return new HitBallResult
        {
            HitType = hitType,
            IsFair = isFair,
            IsCaughtInAir = false,
            BaseCount = EstimateBaseCountByDistance(hitPoint),
            IsHomeRun = false
        };
    }

    private HitBallResult BuildNoCollisionResult(Vector3 lastPos, HitType hitType)
    {
        bool isFair = IsFairDirection(lastPos - Vector3.zero);

        return new HitBallResult
        {
            HitType = hitType,
            IsFair = isFair,
            IsCaughtInAir = false,
            BaseCount = 0,
            IsHomeRun = false
        };
    }

    private bool IsFairDirection(Vector3 fromHome)
    {
        Vector3 dir = new Vector3(fromHome.x, 0f, fromHome.z).normalized;
        if (dir.sqrMagnitude < 0.0001f)
            return true;

        float angle = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);
        return Mathf.Abs(angle) <= _foulLineAngle;
    }

    private int EstimateBaseCountByDistance(Vector3 landingPoint)
    {
        float dist = new Vector3(landingPoint.x, 0f, landingPoint.z).magnitude;

        if (dist < 40f) return 1;
        if (dist < 70f) return 2;
        if (dist < 100f) return 3;
        return 4;
    }
}
