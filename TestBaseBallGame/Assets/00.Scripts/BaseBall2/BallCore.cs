// Scripts/Game/Baseball/Ball/BallCore.cs
using System.Collections.Generic;
using UnityEngine;
using Bash.Framework.Node;
using Bash.Framework.Core;

namespace Bash.Game.Baseball.Ball
{
    /// <summary>
    /// 실제 공 GameObject에 붙는 코어 컴포넌트.
    /// - Rigidbody/Collider를 이용해 물리 움직임을 관리하면서
    ///   내부적으로 BallRuntimeState를 업데이트하고 Behaviour/Skill을 호출한다.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class BallCore : BashNode
    {
        [Header("References")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _collider;

        private BallBehaviourSO _behaviour;
        private readonly List<BallSkillSO> _skills = new();

        private BallRuntimeState _state;
        private BallSpawnContext _context;

        private bool _initialized;

        /// <summary>현재 런타임 상태(읽기 전용).</summary>
        public BallRuntimeState State => _state;

        /// <summary>스폰 시 전달받은 컨텍스트(읽기 전용).</summary>
        public BallSpawnContext SpawnContext => _context;

        protected override void Awake()
        {
            base.Awake();

            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();

            if (_collider == null)
                _collider = GetComponent<Collider>();
        }

        /// <summary>
        /// BallFactoryModule에서 공을 생성한 직후 호출해주는 초기화 메서드.
        /// </summary>
        public void Setup(in BallSpawnContext context, BallBehaviourSO behaviour, IList<BallSkillSO> skills)
        {
            _context = context;
            _behaviour = behaviour;

            _skills.Clear();
            if (skills != null)
            {
                for (int i = 0; i < skills.Count; i++)
                {
                    if (skills[i] != null)
                        _skills.Add(skills[i]);
                }
            }

            // 런타임 상태 초기화
            _state = new BallRuntimeState();
            _behaviour?.Initialize(ref _state, in _context);

            // 스킬 초기화
            for (int i = 0; i < _skills.Count; i++)
            {
                _skills[i].OnBallSpawned(ref _state, in _context);
            }

            // Rigidbody에 초기 상태 적용
            if (_rigidbody != null)
            {
                _rigidbody.position = _state.Position;
                _rigidbody.linearVelocity = _state.Velocity;
            }

            _initialized = true;
        }

        private void FixedUpdate()
        {
            if (!_initialized)
                return;

            float dt = Time.fixedDeltaTime;

            // Rigidbody의 현재 상태를 런타임 상태에 반영
            if (_rigidbody != null)
            {
                _state.Position = _rigidbody.position;
                _state.Velocity = _rigidbody.linearVelocity;
            }

            _state.LifeTime += dt;

            // Behaviour 업데이트
            _behaviour?.UpdateBehaviour(ref _state, dt);

            // Skill 업데이트
            for (int i = 0; i < _skills.Count; i++)
            {
                _skills[i].OnBallUpdated(ref _state, dt);
            }

            // 상태를 Rigidbody에 되적용
            if (_rigidbody != null)
            {
                _rigidbody.position = _state.Position;
                _rigidbody.linearVelocity = _state.Velocity;
            }

            if (_state.MarkForDespawn)
            {
                Despawn();
            }
        }

        /// <summary>
        /// 공의 Phase를 바꾸고, 스킬에 Phase 변경을 알린다.
        /// (룰/주루/수비 모듈에서 호출할 수도 있음)
        /// </summary>
        public void SetPhase(BallPhase newPhase)
        {
            if (!_initialized) return;
            if (_state.Phase == newPhase) return;

            var previous = _state.Phase;
            _state.Phase = newPhase;

            for (int i = 0; i < _skills.Count; i++)
            {
                _skills[i].OnPhaseChanged(ref _state, previous, newPhase);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_initialized) return;
            if (_collider == null) return;

            var other = collision.collider;
            var hitPoint = collision.GetContact(0).point;

            // BeforeCollision
            for (int i = 0; i < _skills.Count; i++)
            {
                _skills[i].OnBeforeCollision(ref _state, other, hitPoint);
            }

            // 여기서 물리/룰 처리(예: 필드 충돌, 벽/지면 판정 등)는
            // 별도 시스템에서 이루어질 수 있다.
            // BallCore는 최소한으로만 개입.

            // AfterCollision
            for (int i = 0; i < _skills.Count; i++)
            {
                _skills[i].OnAfterCollision(ref _state, other, hitPoint);
            }

            if (_state.MarkForDespawn)
            {
                Despawn();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_initialized) return;

            var hitPoint = transform.position;

            for (int i = 0; i < _skills.Count; i++)
            {
                _skills[i].OnBeforeCollision(ref _state, other, hitPoint);
            }

            for (int i = 0; i < _skills.Count; i++)
            {
                _skills[i].OnAfterCollision(ref _state, other, hitPoint);
            }

            if (_state.MarkForDespawn)
            {
                Despawn();
            }
        }

        /// <summary>
        /// 공을 디스폰시킨다. (기본 구현: Destroy)
        /// 나중에 풀링을 붙이고 싶다면 여기만 수정하면 된다.
        /// </summary>
        public void Despawn()
        {
            if (!_initialized) return;

            // 스킬에 Despawn 알림
            for (int i = 0; i < _skills.Count; i++)
            {
                _skills[i].OnBallDespawned(in _state);
            }

            _initialized = false;

            // TODO: PoolManager와 연동할 예정이면 Destroy 대신 풀 반환 호출
            Destroy(gameObject);
        }
    }
}
