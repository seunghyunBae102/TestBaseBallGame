using System.Collections.Generic; // HashSet, List 사용
using UnityEngine;
using Bash.Framework.Managers;

namespace Bash.Framework.Core
{
    public class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance { get; private set; }
        public EventHub Events { get; } = new EventHub();

        // --- 1. 계층 구조 및 노드 저장소 ---

        [field: SerializeField] public ActorNode SystemRoot { get; private set; }
        [field: SerializeField] public ActorNode WorldRoot { get; private set; }

        // [구현] 모든 활성 노드를 추적하는 저장소 (중복 방지를 위해 HashSet 사용)
        private readonly HashSet<ActorNode> _allNodes = new HashSet<ActorNode>();

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeSystem();
            InitializeWorld();
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            // 시스템 -> 월드 순으로 로직 틱 전파
            if (SystemRoot != null) SystemRoot.OnTick(dt);
            if (WorldRoot != null) WorldRoot.OnTick(dt);
        }

        // --- 2. 초기화 로직 ---

        private void InitializeSystem()
        {
            if (SystemRoot == null)
            {
                var go = new GameObject("SystemRoot");
                go.transform.SetParent(transform);
                SystemRoot = go.AddComponent<ActorNode>();
            }

            // 핵심 매니저 부착 (순서: Pool -> Timer -> Sound)
            if (SystemRoot.GetComponent<PoolManager>() == null) SystemRoot.gameObject.AddComponent<PoolManager>();
            if (SystemRoot.GetComponent<TimerManager>() == null) SystemRoot.gameObject.AddComponent<TimerManager>();
            if (SystemRoot.GetComponent<SoundManager>() == null) SystemRoot.gameObject.AddComponent<SoundManager>();

            SystemRoot.InitializeComponents();
        }

        private void InitializeWorld()
        {
            if (WorldRoot == null)
            {
                var go = new GameObject("WorldRoot");
                go.transform.SetParent(transform);
                WorldRoot = go.AddComponent<ActorNode>();
            }
            WorldRoot.InitializeComponents();
        }

        // --- 3. 노드 관리 및 검색 (구현됨) ---

        /// <summary>
        /// ActorNode가 Awake/Init 될 때 호출되어 관리 목록에 추가합니다.
        /// </summary>
        public void RegisterNode(ActorNode node)
        {
            if (node != null)
            {
                _allNodes.Add(node);
            }
        }

        /// <summary>
        /// ActorNode가 파괴될 때 호출되어 관리 목록에서 제거합니다.
        /// </summary>
        public void UnregisterNode(ActorNode node)
        {
            if (node != null)
            {
                _allNodes.Remove(node);
            }
        }

        /// <summary>
        /// 특정 타입의 액터 노드를 찾아 반환합니다. (가장 먼저 발견된 하나)
        /// 예: GameRoot.Instance.FindNode<BallNode>();
        /// </summary>
        public T FindNode<T>() where T : ActorNode
        {
            foreach (var node in _allNodes)
            {
                // 'is' 키워드는 상속 관계까지 체크하므로 유연합니다.
                if (node is T t) return t;
            }
            return null;
        }

        /// <summary>
        /// 특정 타입의 모든 액터 노드를 리스트로 반환합니다.
        /// 예: GameRoot.Instance.FindNodes<FielderPawn>(); // 모든 수비수 찾기
        /// </summary>
        public List<T> FindNodes<T>() where T : ActorNode
        {
            var list = new List<T>();
            foreach (var node in _allNodes)
            {
                if (node is T t) list.Add(t);
            }
            return list;
        }

        // --- 4. 헬퍼 메서드 ---

        public T GetManager<T>() where T : ActorCompo
        {
            if (SystemRoot == null) return null;
            return SystemRoot.GetCompo<T>();
        }
    }
}