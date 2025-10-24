using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GetCompoParentSample<T> : BaseGameCompo,IGetCompoParent<T> where T : IGetCompoParent<T>
{
    protected Dictionary<Type,IGetCompoable<T>> _components = new Dictionary<Type, IGetCompoable<T>>();

    // public EventBus EventBus { get; private set; }
    protected virtual void Awake()
    {
        //EventBus = new EventBus(); // EventBus 초기화를 Awake에서 수행

        Init();

    }

    public void RegisterEvents()
    {
        foreach (var compo in _components.Values)
        {
            // this를 T로 캐스팅하여 전달
            if (this is T parent)
            {
                compo.RegisterEvents(parent);
            }
        }
    }
    
    public void UnregisterEvents()
    {
        foreach (var compo in _components.Values)
        {
            // this를 T로 캐스팅하여 전달
            if (this is T parent)
            {
                compo.UnregisterEvents(parent);
            }
        }
    }


    public virtual void AddCompoDic(Type type, IGetCompoable<T> compo)
    {
        if(!_components.ContainsKey(type))
        _components.Add(type, compo);
    }

    public virtual void RemoveCompoDic(Type type)
    {
        if (_components.ContainsKey(type))
            _components.Remove(type);
    }
    public virtual void AddRealCompo<K>() where K : Component,IGetCompoable<T>
    {
        K instance = gameObject.AddComponent<K>();
        _components.Add(instance.GetType(), instance);
    }
    public virtual K GetCompo<K>(bool isIncludeChild = false) where K : Component, IGetCompoable<T>
    {
        if (_components.TryGetValue(typeof(K), out var component))
        {
            return (K)component;
        }

        if (isIncludeChild == false) return default;

        Type findType = _components.Keys.FirstOrDefault(type => type.IsSubclassOf(typeof(K)));
        
        if (findType != null)
            return (K)_components[findType];

        return default;
    }


    public void ClearCompoDic()
    {
        _components.Clear();
    }

    public IGetCompoable<T> GetCompo(Type type)
    {
        return _components.TryGetValue(type, out var component) ? component : default;
    }

    public U GetCompo<U>() where U : IGetCompoable<T>
    {
        if (_components.TryGetValue(typeof(U), out var component) && component is U result)
        {
            return result;
        }
        return default;
    }

    public bool HasCompo(Type type)
    {
        return _components.ContainsKey(type);
    }

    public bool HasCompo<U>() where U : IGetCompoable<T>
    {
        return _components.ContainsKey(typeof(U));
    }

    public int CompoCount()
    {
        return _components.Count;
    }

    public void ForEachCompo(Action<IGetCompoable<T>> action)
    {
        foreach (var component in _components.Values)
        {
            action(component);
        }
    }

    public void ForEachCompo<U>(Action<U> action) where U : IGetCompoable<T>
    {
        foreach (var component in _components.Values)
        {
            if (component is U typedComponent)
            {
                action(typedComponent);
            }
        }
    }

    public virtual void Init()
    {

        IGetCompoable<T>[] babies = GetComponentsInChildren<IGetCompoable<T>>(true);
        for (int i = 0; i < babies.Length; i++)
        {
            babies[i].Init(this is T parent ? parent : default);
        }

        ILifeCycleable<T>[] babies2 = GetComponentsInChildren<ILifeCycleable<T>>(true);
        for (int i = 0; i < babies2.Length; i++)
        {
            babies2[i].Init(this is T parent ? parent : default);
        }

        RegisterEvents();
    }
}
