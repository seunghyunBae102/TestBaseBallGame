// Scripts/Framework/Core/EventHub.cs
using System;
using System.Collections.Generic;

namespace Bash.Framework.Core
{
    /// <summary>
    /// 간단한 타입 기반 EventBus.
    /// - Subscribe&lt;T&gt;(Action&lt;T&gt;)
    /// - Unsubscribe&lt;T&gt;(Action&lt;T&gt;)
    /// - Publish&lt;T&gt;(T)
    /// </summary>
    public class EventHub
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();

        /// <summary>
        /// 타입 T에 대한 구독 추가.
        /// </summary>
        public void Subscribe<T>(Action<T> handler)
        {
            if (handler == null) return;

            var type = typeof(T);

            if (_handlers.TryGetValue(type, out var existing))
            {
                _handlers[type] = (Action<T>)existing + handler;
            }
            else
            {
                _handlers[type] = handler;
            }
        }

        /// <summary>
        /// 타입 T에 대한 구독 제거.
        /// </summary>
        public void Unsubscribe<T>(Action<T> handler)
        {
            if (handler == null) return;

            var type = typeof(T);

            if (_handlers.TryGetValue(type, out var existing))
            {
                var current = (Action<T>)existing;
                current -= handler;

                if (current == null)
                {
                    _handlers.Remove(type);
                }
                else
                {
                    _handlers[type] = current;
                }
            }
        }

        /// <summary>
        /// 타입 T 이벤트 발행.
        /// </summary>
        public void Publish<T>(T evt)
        {
            var type = typeof(T);

            if (_handlers.TryGetValue(type, out var existing))
            {
                var handler = (Action<T>)existing;
                handler?.Invoke(evt);
            }
        }

        /// <summary>
        /// 모든 구독 제거 (씬 전환 등에서 사용).
        /// </summary>
        public void Clear()
        {
            _handlers.Clear();
        }
    }
}
