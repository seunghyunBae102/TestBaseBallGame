//// Scripts/Framework/Core/ActorCompo.cs
//using UnityEngine;

//namespace Bash.Framework.Core
//{
//    /// <summary>
//    /// [Core] 기능 모듈의 베이스.
//    /// - MonoBehaviour 상속: 인스펙터 편집 가능.
//    /// - Update 미사용: GameRoot -> ActorNode -> OnTick 흐름을 따름 (제어권 확보).
//    /// </summary>
//    public abstract class ActorCompo : MonoBehaviour
//    {
//        public ActorNode Owner { get; private set; }

//        // 로직 활성화 여부 (Inspector의 체크박스와 연동됨)
//        public bool IsActive => gameObject.activeInHierarchy && enabled;

//        protected EventHub Events => GameRoot.Instance.Events;

//        // --- 초기화 ---

//        public void Init(ActorNode owner)
//        {
//            Owner = owner;
//            OnInit();
//        }

//        /// <summary>
//        /// 실제 초기화 로직 (Awake 대신 사용 권장).
//        /// Owner가 연결된 직후 호출됨.
//        /// </summary>
//        protected virtual void OnInit() { }

//        // --- 로직 ---

//        /// <summary>
//        /// 프레임워크가 호출하는 업데이트 함수.
//        /// (Unity Update 아님)
//        /// </summary>
//        public virtual void OnTick(float dt) { }

//        // --- 안전장치 (Unity Editor 기능) ---

//#if UNITY_EDITOR
//        protected virtual void Reset()
//        {
//            // 컴포넌트를 추가했을 때 자동으로 ActorNode를 찾으려 시도
//            if (GetComponent<ActorNode>() == null)
//            {
//                Debug.LogWarning($"[ActorCompo] '{name}' 오브젝트에 ActorNode가 없습니다. 제대로 동작하려면 ActorNode가 필요합니다.");
//            }
//        }
//#endif
//    }
//}