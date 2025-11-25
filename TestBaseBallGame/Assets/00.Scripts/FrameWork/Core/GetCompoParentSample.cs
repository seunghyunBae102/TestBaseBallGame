using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class GetCompoParentSample<T> : BaseGameCompo,IGetCompoParent<T> where T : IGetCompoParent<T>
{
    protected Dictionary<(Type type,string id),IGetCompoable<T>> _components = new Dictionary<(Type,string), IGetCompoable<T>>();

    private readonly List<ILifeCycleable<T>> _lifeCycles = new();

    public GameEventBus EventBus { get; private set; } = new GameEventBus();
    protected virtual void Awake()
    {
        

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

    public virtual void RemoveCompoDic(Type type,string name ="")
    {
        if (_components.ContainsKey(type))
            _components.Remove(type);
    }
    public virtual void RemoveCompoDic(Type type)
    {
        if (_components.ContainsKey(type))
            _components.Remove(type);
    }
    public virtual void AddRealCompo<K>(string name) where K : Component,IGetCompoable<T>
    {
        K instance = gameObject.AddComponent<K>();
        _components.Add((instance.GetType(),name), instance);
    }
    public virtual K GetCompo<K>(bool isIncludeChild = false) where K : Component, IGetCompoable<T>
    {
           K component = _components.FirstOrDefault(kv => kv.Key.type == typeof(T)).Value as K;

        if(component)
            return (K)component;

        if (isIncludeChild == false) return default;

        foreach (var ((type, r), s) in _components)
        {
            if (r == (name ?? "") && typeof(T).IsAssignableFrom(type))
                return (K)s;
        }

        return default;
    }
    public virtual K GetCompo<K>(string name = "", bool isIncludeChild = false) where K : Component, IGetCompoable<T>
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

    public virtual IGetCompoable<T> GetCompo(Type type)
    {
        return _components.TryGetValue(type, out var component) ? component : default;
    }

    public virtual U GetCompo<U>() where U : IGetCompoable<T>
    {
        if (_components.TryGetValue(typeof(U), out var component) && component is U result)
        {
            return result;
        }
        return default;
    }
    public virtual U GetOrAddCompo<U>() where U : Component,IGetCompoable<T>
    {
        if (_components.TryGetValue(typeof(U), out var component) && component is U result)
        {
            return result;
        }
        
        AddRealCompo<U>();

        return GetCompo<U>();
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

        _lifeCycles.Clear();
        ILifeCycleable<T>[] babies2 = GetComponentsInChildren<ILifeCycleable<T>>(true);
        for (int i = 0; i < babies2.Length; i++)
        {
            var lc = babies2[i];
            lc.Init(this is T parent ? parent : default);
            _lifeCycles.Add(lc);
        }

        RegisterEvents();


        foreach (var lc in _lifeCycles)
            lc.AfterInit();
    }

    protected virtual void Update()
    {
        float dt = Time.deltaTime;
        foreach (var lc in _lifeCycles)
            lc.Tick(dt);
    }

    // FixedUpdate Tick
    protected virtual void FixedUpdate()
    {
        float fdt = Time.fixedDeltaTime;
        foreach (var lc in _lifeCycles)
            lc.TickFixed(fdt);
    }

    // 삭제 직전 콜백
    protected virtual void OnDestroy()
    {
        foreach (var lc in _lifeCycles)
            lc.BeforeDestroy();

        UnregisterEvents();
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
