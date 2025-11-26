//// Scripts/Game/Baseball/Match/Modules/HitBallModule.cs
//using UnityEngine;

///// <summary>
///// 타구 궤적을 시뮬레이션하고 지면/수비수/펜스와의 충돌로 HitBallResult를 만든다.
///// </summary>
//public class HitBallModule : Module
//{
//    [Header("Behaviour & Skills")]
//    [SerializeField] private BallBehaviourSO _baseBehaviour;   // Fly / Ground 등
//    [SerializeField] private BallSkillSO[] _skills;          // 타구 스킬

//    [Header("Visual")]
//    [SerializeField] private Transform _ballVisual;

//    [Header("Collision")]
//    [SerializeField] private LayerMask _groundMask;
//    [SerializeField] private LayerMask _fenceMask;
//    [SerializeField] private LayerMask _fielderMask;

//    [Header("Settings")]
//    [SerializeField] private float _maxFlightTime = 5f;
//    [SerializeField] private Vector3 _gravity = new(0, -9.81f, 0);
//    [SerializeField] private float _foulLineX = 10f; // 단순 페어/파울 기준

//    private BallState _state;
//    private BallContext _ctx;
//    private bool _isActive;
//    private HitType _hitType;

//    private Vector3 _prevPos;

//    protected override void Tick(float dt)
//    {
//        if (!_isActive || _baseBehaviour == null) return;

//        _prevPos = _state.position;

//        _baseBehaviour.SimulateStep(ref _state, in _ctx, dt);
//        if (_skills != null)
//        {
//            foreach (var s in _skills)
//                s?.Modify(ref _state, in _ctx, dt);
//        }

//        if (_ballVisual) _ballVisual.position = _state.position;
//        else transform.position = _state.position;

//        _state.time += dt;

//        if (_state.time >= _maxFlightTime)
//        {
//            FinalizeAsDeadBall();
//            return;
//        }

//        CheckCollisions();
//    }

//    /// <summary>
//    /// 타구 시작. BatSwing 모듈(아직 안 만듦)에서 호출.
//    /// </summary>
//    public void BeginHit(Vector3 startPos, Vector3 initialVelocity,
//                         HitType hitType,
//                         BallBehaviourSO behaviour,
//                         BallSkillSO[] skills = null)
//    {
//        _baseBehaviour = behaviour;
//        _skills = skills;
//        _hitType = hitType;

//        _ctx = new BallContext
//        {
//            startPos = startPos,
//            targetPos = startPos + initialVelocity.normalized * 50f,
//            gravity = _gravity,
//            mass = 1f,
//            obstacleMask = _groundMask | _fenceMask | _fielderMask
//        };

//        _state = new BallState
//        {
//            position = startPos,
//            velocity = initialVelocity,
//            time = 0f,
//            isAlive = true
//        };

//        _baseBehaviour.Initialize(ref _state, in _ctx);

//        if (_ballVisual) _ballVisual.position = _state.position;
//        else transform.position = _state.position;

//        _prevPos = _state.position;
//        _isActive = true;
//    }

//    private void CheckCollisions()
//    {
//        Vector3 currPos = _state.position;
//        Vector3 delta = currPos - _prevPos;
//        float dist = delta.magnitude;
//        if (dist <= 0f) return;

//        Vector3 dir = delta / dist;

//        // 1) 수비수 캐치
//        if (Physics.Raycast(_prevPos, dir, out RaycastHit fielderHit, dist, _fielderMask, QueryTriggerInteraction.Ignore))
//        {
//            bool isCaughtInAir = _hitType != HitType.GroundBall;

//            var result = new HitBallResult
//            {
//                HitType = _hitType,
//                IsFair = true, // 수비수 위치를 페어로 가정
//                IsCaughtInAir = isCaughtInAir,
//                BaseCount = 0,
//                IsHomeRun = false
//            };

//            Events?.Publish(result);
//            _isActive = false;
//            return;
//        }

//        // 2) 펜스/스탠드
//        if (Physics.Raycast(_prevPos, dir, out RaycastHit fenceHit, dist, _fenceMask, QueryTriggerInteraction.Ignore))
//        {
//            bool isHomeRun = fenceHit.point.y > 2f; // 예시 기준

//            var result = new HitBallResult
//            {
//                HitType = HitType.HomeRun,
//                IsFair = true,
//                IsCaughtInAir = false,
//                BaseCount = isHomeRun ? 4 : 2,
//                IsHomeRun = isHomeRun
//            };

//            Events?.Publish(result);
//            _isActive = false;
//            return;
//        }

//        // 3) 지면 (잔디/내야)
//        if (Physics.Raycast(_prevPos, dir, out RaycastHit groundHit, dist, _groundMask, QueryTriggerInteraction.Ignore))
//        {
//            bool isFair = IsFairTerritory(groundHit.point);
//            int baseCount = 1;
//            if (_hitType == HitType.LineDrive) baseCount = 2;

//            var result = new HitBallResult
//            {
//                HitType = _hitType,
//                IsFair = isFair,
//                IsCaughtInAir = false,
//                BaseCount = isFair ? baseCount : 0,
//                IsHomeRun = false
//            };

//            Events?.Publish(result);
//            _isActive = false;
//            return;
//        }
//    }

//    private void FinalizeAsDeadBall()
//    {
//        _isActive = false;

//        var result = new HitBallResult
//        {
//            HitType = _hitType,
//            IsFair = false,
//            IsCaughtInAir = false,
//            BaseCount = 0,
//            IsHomeRun = false
//        };

//        Events?.Publish(result);
//    }

//    private bool IsFairTerritory(Vector3 point)
//    {
//        return Mathf.Abs(point.x) < _foulLineX;
//    }
//}
