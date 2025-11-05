using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class GetCompoParentSample<T> : BaseGameCompo,IGetCompoParent<T> where T : IGetCompoParent<T>
{
    protected Dictionary<(Type type,string id),IGetCompoable<T>> _components = new Dictionary<(Type,string), IGetCompoable<T>>();

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


    public virtual void AddCompoDic(Type type, IGetCompoable<T> compo, string name = "")
    {
        if(!HasCompo(type))
        _components.Add((type,name), compo);
    }

    public virtual void RemoveCompoDic<K>(string name ="") where K : IGetCompoable<T>
    {
        if (HasCompo(typeof(K)))
        {
            GetCompo<K>(name)?.UnregisterEvents(this is T parent ? parent : default);
            _components.Remove((typeof(K),name));
        }
    }
    public virtual void RemoveCompoDic<K>() where K : IGetCompoable<T>
    {
        if (HasCompo(typeof(K)))
        {
            foreach (var ((type, r), s) in _components)
            {
                if (typeof(K).IsAssignableFrom(type))
                {
                    s?.UnregisterEvents(this is T parent ? parent : default);
                    _components.Remove((type, r));
                    return;
                }
            }
        }
    }
    //public virtual void RemoveCompoDic(Type type)
    //{
    //    if (_components.ContainsKey(type))
    //        _components.Remove(type);
    //}
    public virtual void AddRealCompo<K>(string name) where K : Component, IGetCompoable<T>
    {
        K instance = gameObject.AddComponent<K>();
        _components.Add((instance.GetType(),name), instance);
    }
    public virtual K GetCompo<K>(bool isIncludeChild = false) where K : IGetCompoable<T>
    {
        foreach (var ((type, r), s) in _components)
        {
            if (typeof(T) == type)
                return (K)s;
        }

        if (isIncludeChild == false) return default;

        foreach (var ((type, r), s) in _components)
        {
            if (typeof(T).IsAssignableFrom(type))
                return (K)s;
        }

        return default;
    }
    public virtual K GetCompo<K>(string name, bool isIncludeChild = false) where K : IGetCompoable<T>
    {
        if (_components.TryGetValue((typeof(K), name ?? ""), out var component))
        {
            return (K)component;
        }

        if (isIncludeChild == false) return default;

        foreach (var ((type, r), s) in _components)
        {
            if (typeof(T).IsAssignableFrom(type))
                return (K)s;
        }

        return default;
    }

    public void ClearCompoDic()
    {
        _components.Clear();
    }

    public virtual U GetOrAddCompo<U>(string name ="") where U : Component,IGetCompoable<T>
    {
        U compo = GetCompo<U>(name);

        if(compo == null)
        AddRealCompo<U>(name);

        return GetCompo<U>(name);
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

    public void AddCompoDic(Type type, IGetCompoable<T> compo)
    {
        throw new NotImplementedException();
    }

    public bool HasCompo(Type type)
    {
        foreach (var ((type1, r), s) in _components)
        {
            if (type.IsAssignableFrom(type1))
                return true;
        }
        return false;
    }

    public bool HasCompo(Type type, string name)
    {
        foreach (var ((type1, r), s) in _components)
        {
            if (r == (name ?? "") && type.IsAssignableFrom(type1))
                return true;
        }
        return false;
    }

    public bool HasCompo(string name)
    {
        foreach (var ((type1, r), s) in _components)
        {
            if (r == (name ?? ""))
                return true;
        }
        return false;
    }
}
