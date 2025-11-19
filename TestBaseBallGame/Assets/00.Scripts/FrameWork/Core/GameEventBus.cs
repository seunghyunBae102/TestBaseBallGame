using System;
using System.Collections.Generic;

public interface IGameEventListener<in T> where T : IGameEvent
{
    void OnEvent(T e);
}

public interface IGameEventBus
{
    void Publish<T>(T e) where T : IGameEvent;
    void Subscribe<T>(IGameEventListener<T> listener) where T : IGameEvent;
    void Unsubscribe<T>(IGameEventListener<T> listener) where T : IGameEvent;
}

public class GameEventBus : IGameEventBus
{
    private readonly Dictionary<Type, List<object>> _listeners = new();

    public void Subscribe<T>(IGameEventListener<T> listener) where T : IGameEvent
    {
        var type = typeof(T);
        if (!_listeners.TryGetValue(type, out var list))
        {
            list = new List<object>();
            _listeners[type] = list;
        }

        if (!list.Contains(listener))
            list.Add(listener);
    }

    public void Unsubscribe<T>(IGameEventListener<T> listener) where T : IGameEvent
    {
        var type = typeof(T);
        if (_listeners.TryGetValue(type, out var list))
            list.Remove(listener);
    }

    public void Publish<T>(T e) where T : IGameEvent
    {
        var type = typeof(T);
        if (!_listeners.TryGetValue(type, out var list))
            return;

        // 복사본 띄워서 루프 중 Subscribe/Unsubscribe해도 안전하게
        var snapshot = list.ToArray();
        foreach (var obj in snapshot)
            ((IGameEventListener<T>)obj).OnEvent(e);
    }
}
