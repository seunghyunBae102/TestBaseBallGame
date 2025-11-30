//// Scripts/Framework/Core/BashNode.cs
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Bash.Framework.Core
//{
//    /// <summary>
//    /// [Core] GameObject의 논리적 래퍼.
//    /// - 역할: 모듈(Module)들을 담는 그릇이자, 계층 구조의 노드.
//    /// - 특징: GetComponent를 쓰지 않고 내부 Dictionary로 빠르게 모듈을 조회함.
//    /// </summary>
//    public class BashNode : MonoBehaviour
//    {
//        // 모듈 캐싱 저장소 (Type -> Module)
//        private readonly Dictionary<Type, Module> _moduleCache = new Dictionary<Type, Module>();
//        private bool _isInitialized = false;

//        protected virtual void Awake()
//        {
//            InitializeModules();
//            // 생성 시 전역 레지스트리에 등록 (검색 편의성)
//            GameRoot.Instance?.RegisterNode(this);
//        }

//        protected virtual void OnDestroy()
//        {
//            GameRoot.Instance?.UnregisterNode(this);
//        }

//        /// <summary>
//        /// 내 자식 오브젝트들에 있는 모든 Module을 찾아서 등록하고 초기화(Init)를 실행.
//        /// 동적으로 컴포넌트가 추가되었을 때 수동 호출 가능.
//        /// </summary>
//        public void InitializeModules()
//        {
//            _moduleCache.Clear();

//            // 비활성화된 자식까지 포함하여 모듈 수집
//            var modules = GetComponentsInChildren<Module>(true);
//            foreach (var module in modules)
//            {
//                var type = module.GetType();

//                // 중복 방지 (같은 타입이 여러 개면 첫 번째 것만 등록)
//                if (!_moduleCache.ContainsKey(type))
//                {
//                    _moduleCache.Add(type, module);
//                }

//                // 모듈에게 주인(Owner)이 나임을 알려줌
//                module.Init(this);
//            }
//            _isInitialized = true;
//        }

//        // --- 모듈 조회 (GetComponent 대체) ---

//        /// <summary>
//        /// 가장 빠른 모듈 조회. 없으면 null 반환.
//        /// </summary>
//        public T GetModule<T>() where T : Module
//        {
//            if (!_isInitialized) InitializeModules();
//            return _moduleCache.TryGetValue(typeof(T), out var module) ? (T)module : null;
//        }

//        /// <summary>
//        /// 특정 모듈이 있는지 확인.
//        /// </summary>
//        public bool HasModule<T>() where T : Module
//        {
//            if (!_isInitialized) InitializeModules();
//            return _moduleCache.ContainsKey(typeof(T));
//        }

//        /// <summary>
//        /// [AI 안전장치] 모듈이 없으면 에러 로그를 띄워 문제 파악을 도움.
//        /// </summary>
//        public T GetModuleSafe<T>() where T : Module
//        {
//            var module = GetModule<T>();
//            if (module == null)
//                Debug.LogError($"[BashNode Error] '{name}' 노드에 필수 모듈 '{typeof(T).Name}'이(가) 없습니다!");
//            return module;
//        }

//        // --- 계층 구조 (Unity Transform 래핑) ---

//        /// <summary>
//        /// 내 부모 BashNode. (없거나 일반 GameObject면 null)
//        /// </summary>
//        public BashNode Parent
//        {
//            get
//            {
//                if (transform.parent == null) return null;
//                return transform.parent.GetComponent<BashNode>();
//            }
//        }

//        /// <summary>
//        /// 내 직속 자식 BashNode들을 리스트로 가져옴.
//        /// (실시간으로 Transform을 순회하여 가져오므로 항상 정확함)
//        /// </summary>
//        public List<BashNode> GetChildren()
//        {
//            var list = new List<BashNode>();
//            foreach (Transform childTr in transform)
//            {
//                var node = childTr.GetComponent<BashNode>();
//                if (node != null) list.Add(node);
//            }
//            return list;
//        }

//        /// <summary>
//        /// 이 노드를 다른 노드의 자식으로 붙임. (Unreal의 AttachToActor 유사)
//        /// </summary>
//        public void AttachTo(BashNode parentNode)
//        {
//            if (parentNode != null)
//                transform.SetParent(parentNode.transform, false); // false: 로컬 좌표 유지
//            else
//                transform.SetParent(null); // 최상위로 이동
//        }

//        /// <summary>
//        /// 부모로부터 분리.
//        /// </summary>
//        public void Detach()
//        {
//            transform.SetParent(null);
//        }
//    }
//}