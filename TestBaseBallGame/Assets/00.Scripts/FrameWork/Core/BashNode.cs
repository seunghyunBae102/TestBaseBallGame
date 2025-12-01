// ---------------------------------------------------------
// 1. ActorNode.cs (가상 계층 노드)
// ---------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bash.Framework.Core
{
    public class ActorNode : MonoBehaviour
    {
        // 모듈 캐싱 (Dictionary)
        private readonly Dictionary<Type, ActorCompo> _components = new Dictionary<Type, ActorCompo>();
        private bool _isInitialized = false;

        // 가상 계층 구조
        public ActorNode ParentNode { get; private set; }
        private readonly List<ActorNode> _childNodes = new List<ActorNode>();

        protected virtual void Awake()
        {
            InitializeComponents();
            GameRoot.Instance?.RegisterNode(this);
        }

        protected virtual void OnDestroy() => GameRoot.Instance?.UnregisterNode(this);

        public void InitializeComponents()
        {
            if (_isInitialized) return;
            var compos = GetComponents<ActorCompo>();
            foreach (var c in compos) RegisterCompo(c);
            _isInitialized = true;
        }

        private void RegisterCompo(ActorCompo compo)
        {
            var type = compo.GetType();
            if (!_components.ContainsKey(type)) _components.Add(type, compo);
            compo.Init(this);
        }

        public T GetCompo<T>() where T : ActorCompo
        {
            if (!_isInitialized) InitializeComponents();
            return _components.TryGetValue(typeof(T), out var c) ? (T)c : null;
        }
        //public ActorCompo GetCompo(Type T)
        //{

        //    if (!_isInitialized) InitializeComponents();
        //    return _components.TryGetValue(T, out var c) ? (T)c : null;
        //}

        // 가상 계층 관리
        public void AttachTo(ActorNode parent)
        {
            if (ParentNode != null) ParentNode._childNodes.Remove(this);
            ParentNode = parent;
            if (ParentNode != null) ParentNode._childNodes.Add(this);
            transform.SetParent(parent ? parent.transform : null);
        }

        // 로직 루프 (GameRoot -> Node -> Compo)
        public virtual void OnTick(float dt)
        {
            foreach (var compo in _components.Values)
                if (compo.IsActive && compo.enabled) compo.OnTick(dt);

            for (int i = 0; i < _childNodes.Count; i++)
                _childNodes[i].OnTick(dt);
        }
    }
}

// ---------------------------------------------------------
// 2. ActorCompo.cs (기능 모듈 베이스)
// ---------------------------------------------------------

namespace Bash.Framework.Core
{
    public abstract class ActorCompo : MonoBehaviour
    {
        public ActorNode Owner { get; private set; }
        public bool IsActive => gameObject.activeInHierarchy && enabled;
        protected EventHub Events => GameRoot.Instance.Events;

        public void Init(ActorNode owner)
        {
            Owner = owner;
            OnInit();
        }

        protected virtual void OnInit() { }
        public virtual void OnTick(float dt) { }

        protected virtual void OnDestroy() { }

    }
}



// ---------------------------------------------------------
// 4. EventHub.cs (이벤트 버스)
// ---------------------------------------------------------

namespace Bash.Framework.Core
{
    public class EventHub
    {
        private readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>();

        public void Subscribe<T>(Action<T> h)
        {
            var t = typeof(T);
            if (!_handlers.ContainsKey(t)) _handlers[t] = null;
            _handlers[t] = (Action<T>)_handlers[t] + h;
        }

        public void Unsubscribe<T>(Action<T> h)
        {
            var t = typeof(T);
            if (_handlers.ContainsKey(t))
            {
                var d = (Action<T>)_handlers[t];
                d -= h;
                if (d == null) _handlers.Remove(t); else _handlers[t] = d;
            }
        }

        public void Publish<T>(T evt)
        {
            if (_handlers.TryGetValue(typeof(T), out var d)) ((Action<T>)d)?.Invoke(evt);
        }
    }
}