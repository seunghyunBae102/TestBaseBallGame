using UnityEngine;
using Bash.Framework.Managers; // 매니저 접근을 위해 네임스페이스 추가

namespace Bash.Framework.Core
{
    public class GameRoot : MonoBehaviour
    {
        public static GameRoot Instance { get; private set; }
        public EventHub Events { get; } = new EventHub();

        // --- 1. 계층 구조 분리 ---
        // 시스템 루트: Pool, Sound, Timer 등 엔진/인프라 역할
        [field: SerializeField] public ActorNode SystemRoot { get; private set; }

        // 월드 루트: 경기장, 선수, 심판 등 인게임 로직 역할
        [field: SerializeField] public ActorNode WorldRoot { get; private set; }

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeSystem();
            InitializeWorld();
        }

        private void InitializeSystem()
        {
            if (SystemRoot == null)
            {
                var go = new GameObject("SystemRoot");
                go.transform.SetParent(transform);
                SystemRoot = go.AddComponent<ActorNode>();
            }

            // 2. 핵심 매니저 부착 (순서 중요: Pool -> Timer -> Sound)
            // 코드로 직접 붙여서 초기화 순서를 보장합니다.
            if (SystemRoot.GetComponent<PoolManager>() == null) SystemRoot.gameObject.AddComponent<PoolManager>();
            if (SystemRoot.GetComponent<TimerManager>() == null) SystemRoot.gameObject.AddComponent<TimerManager>();
            if (SystemRoot.GetComponent<SoundManager>() == null) SystemRoot.gameObject.AddComponent<SoundManager>();

            // 3. 시스템 즉시 초기화
            // ActorNode.InitializeComponents를 호출하여 매니저들의 OnInit()을 발동시킵니다.
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

            // 월드 루트는 씬에 배치된 오브젝트들에 의해 나중에 채워질 수도 있으므로
            // 여기서 강제 초기화는 하지 않거나, 필요 시 호출합니다.
            WorldRoot.InitializeComponents();
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            // 4. 실행 순서 제어 (Tick Ordering)
            // 시스템(타이머 등)이 먼저 돌고 -> 게임 로직이 돕니다.
            if (SystemRoot != null) SystemRoot.OnTick(dt);
            if (WorldRoot != null) WorldRoot.OnTick(dt);
        }

        // --- 5. 헬퍼 메서드 ---

        public void RegisterNode(ActorNode node)
        {
            // 전체 노드 추적이 필요하다면 구현, 
            // 현재 구조에서는 SystemRoot/WorldRoot 하위로 관리되므로 필수는 아님
        }

        public void UnregisterNode(ActorNode node)
        {
            // 위와 동일
        }

        /// <summary>
        /// 시스템 매니저(SystemRoot 산하)를 쉽고 빠르게 가져옵니다.
        /// 예: GameRoot.Instance.GetManager<PoolManager>().Spawn(...)
        /// </summary>
        public T GetManager<T>() where T : ActorCompo
        {
            if (SystemRoot == null) return null;
            return SystemRoot.GetCompo<T>();
        }
    }
}