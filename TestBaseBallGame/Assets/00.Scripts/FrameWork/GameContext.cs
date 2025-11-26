using System;
using System.Collections.Generic;
using Bash.Framework.Core;

/// <summary>
/// ���� ����/�̺�Ʈ ��긦 �����ϴ� ���ؽ�Ʈ.
/// </summary>
public sealed class GameContext
    {
        private readonly Dictionary<Type, object> _services = new();

        public EventHub Events { get; } = new EventHub();

        /// <summary>
        /// Ÿ�� ��� ���� ��� (ManagerBase���� �ַ� ���).
        /// </summary>
        public void RegisterService(Type type, object instance)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            _services[type] = instance;
        }

        /// <summary>
        /// ���׸� ���� ���. ���� RegisterService&lt;TInterface&gt;(this) ���·� ���.
        /// </summary>
        public void RegisterService<TService>(TService instance) where TService : class
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            _services[typeof(TService)] = instance;
        }

        public TService GetService<TService>() where TService : class
        {
            if (_services.TryGetValue(typeof(TService), out var obj))
                return obj as TService;
            return null;
        }

        public object GetService(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            _services.TryGetValue(type, out var obj);
            return obj;
        }
    }


