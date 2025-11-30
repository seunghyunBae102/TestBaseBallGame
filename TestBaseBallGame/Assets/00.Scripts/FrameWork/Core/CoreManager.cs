using Bash.Framework.Core; // ActorNode, ActorCompo, GameRoot 네임스페이스
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bash.Framework.Managers
{
    // ---------------------------------------------------------
    // 1. Pool Manager (가상 계층 연동 강화)
    // ---------------------------------------------------------

    // 풀링되는 객체가 초기화/해제 시 호출받고 싶다면 구현
    public interface IPoolable
    {
        void OnSpawn();
        void OnDespawn();
    }

    [Serializable]
    public class PoolProfile
    {
        public string Id;
        public GameObject Prefab;
        public int DefaultSize = 10;
    }

    public class PoolManager : ActorCompo
    {
        [SerializeField] private List<PoolProfile> _profiles;

        private readonly Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();
        private readonly Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, Transform> _poolParents = new Dictionary<string, Transform>();

        protected override void OnInit()
        {
            if (_profiles == null) return;
            foreach (var p in _profiles)
            {
                CreatePool(p.Id, p.Prefab, p.DefaultSize);
            }
        }

        public void CreatePool(string id, GameObject prefab, int size)
        {
            if (_pools.ContainsKey(id)) return;

            _pools[id] = new Queue<GameObject>();
            _prefabs[id] = prefab;

            // 계층 구조 정리용 (Unity Hierarchy 시각적 정리)
            var poolRoot = new GameObject($"Pool_{id}").transform;
            poolRoot.SetParent(this.transform);
            _poolParents[id] = poolRoot;

            for (int i = 0; i < size; i++)
            {
                var go = Instantiate(prefab, poolRoot);
                go.SetActive(false);

                // 미리 노드 등록을 해제해둠 (안전장치)
                var node = go.GetComponent<ActorNode>();
                if (node != null) GameRoot.Instance.UnregisterNode(node);

                _pools[id].Enqueue(go);
            }
        }

        /// <summary>
        /// 객체를 스폰하고 가상 계층(ActorNode)에 연결합니다.
        /// </summary>
        /// <param name="parentNode">이 객체가 붙을 가상 부모 노드 (중요)</param>
        public GameObject Spawn(string id, ActorNode parentNode, Vector3 pos, Quaternion rot)
        {
            if (!_pools.ContainsKey(id))
            {
                Debug.LogWarning($"[Pool] ID '{id}' Not Found.");
                return null;
            }

            var queue = _pools[id];
            GameObject go;

            // 1. 큐에서 꺼내거나 새로 생성
            if (queue.Count > 0)
            {
                go = queue.Dequeue();
            }
            else
            {
                go = Instantiate(_prefabs[id], _poolParents[id]);
            }

            // 2. 위치 설정
            go.transform.SetPositionAndRotation(pos, rot);
            go.SetActive(true);

            // 3. [핵심] 가상 계층 연결 (BashNode Hierarchy)
            var node = go.GetComponent<ActorNode>();
            if (node != null)
            {
                // 부모 노드가 지정되었으면 붙이고, 없으면(null) WorldRoot 등 기본값 처리가 필요할 수 있음
                // 여기서는 parentNode가 null이면 루트 없이 동작(혹은 호출자가 책임짐)
                node.AttachTo(parentNode);

                // 만약 ActorCompo들을 재초기화해야 한다면 여기서 수행
                node.InitializeComponents();
            }

            // 4. IPoolable 인터페이스 호출
            var poolable = go.GetComponent<IPoolable>(); // 혹은 GetCompo<IPoolable> 등으로 교체 가능
            poolable?.OnSpawn();

            return go;
        }

        public void Despawn(string id, GameObject go)
        {
            if (go == null) return;

            // 1. IPoolable 해제
            go.GetComponent<IPoolable>()?.OnDespawn();

            // 2. 가상 계층 분리 (부모로부터 제거)
            var node = go.GetComponent<ActorNode>();
            if (node != null)
            {
                node.AttachTo(null); // 트리에서 제거
            }

            // 3. Unity 계층 정리 및 비활성화
            go.SetActive(false);
            if (_poolParents.TryGetValue(id, out var parent))
            {
                go.transform.SetParent(parent);
            }

            // 4. 큐 반납
            if (_pools.ContainsKey(id))
            {
                _pools[id].Enqueue(go);
            }
            else
            {
                Destroy(go); // 풀이 삭제된 경우 파괴
            }
        }
    }

    // ---------------------------------------------------------
    // 2. Timer Manager (OnTick 기반 스케줄러)
    // ---------------------------------------------------------

    public class TimerManager : ActorCompo
    {
        private class Timer
        {
            public float RemainingTime;
            public Action Callback;
        }

        private readonly List<Timer> _activeTimers = new List<Timer>();

        // 반복문 도중 리스트 수정 오류 방지용 버퍼
        private readonly List<Timer> _timersToRemove = new List<Timer>();

        /// <summary>
        /// Unity Update가 아닌 GameRoot의 Tick을 받아 동작합니다.
        /// </summary>
        public override void OnTick(float dt)
        {
            if (_activeTimers.Count == 0) return;

            _timersToRemove.Clear();

            // 역순으로 돌거나 별도 리스트 관리 (여기선 안전하게 별도 리스트 사용 예시)
            foreach (var t in _activeTimers)
            {
                t.RemainingTime -= dt;
                if (t.RemainingTime <= 0)
                {
                    t.Callback?.Invoke();
                    _timersToRemove.Add(t);
                }
            }

            foreach (var t in _timersToRemove)
            {
                _activeTimers.Remove(t);
            }
        }

        public void SetTimer(float time, Action callback)
        {
            _activeTimers.Add(new Timer { RemainingTime = time, Callback = callback });
        }
    }

    // ---------------------------------------------------------
    // 3. Sound Manager (Coroutine 제거 & 의존성 해결)
    // ---------------------------------------------------------

    public class SoundManager : ActorCompo
    {
        [Header("Settings")]
        [SerializeField] private GameObject _sfxPrefab;
        [SerializeField] private string _poolId = "SFX_OneShot";

        private PoolManager _pool;
        private TimerManager _timer;

        // 초기화 시 GameRoot를 통해 다른 매니저들을 가져옵니다.
        protected override void OnInit()
        {
            // 주의: GameRoot에 GetManager<T>가 구현되어 있다고 가정 (다음 단계에서 구현 예정)
            // 혹은 Owner.GetCompo<T>()를 써도 됩니다 (모두 SystemRoot라는 같은 노드에 있다면)

            _pool = Owner.GetCompo<PoolManager>();
            _timer = Owner.GetCompo<TimerManager>();

            if (_pool != null && _sfxPrefab != null)
            {
                _pool.CreatePool(_poolId, _sfxPrefab, 10);
            }
        }

        public void PlaySFX(AudioClip clip, Vector3 pos, float volume = 1.0f)
        {
            if (clip == null || _pool == null) return;

            // SFX는 논리적 부모가 딱히 필요 없으므로 parentNode에 null 혹은 SystemRoot 전달
            var go = _pool.Spawn(_poolId, Owner, pos, Quaternion.identity);

            var source = go.GetComponent<AudioSource>();
            if (source != null)
            {
                source.clip = clip;
                source.volume = volume;
                source.Play();

                // Coroutine 대신 TimerManager 사용
                if (_timer != null)
                {
                    _timer.SetTimer(clip.length, () =>
                    {
                        // 반납 시점에도 매니저가 살아있는지 체크
                        if (_pool != null) _pool.Despawn(_poolId, go);
                    });
                }
                else
                {
                    // Timer가 없으면 그냥 놔두거나 즉시 반납해야 함 (예외처리)
                    Debug.LogWarning("TimerManager missing, SFX won't despawn automatically.");
                }
            }
            else
            {
                // AudioSource가 없으면 즉시 반납
                _pool.Despawn(_poolId, go);
            }
        }
    }
}