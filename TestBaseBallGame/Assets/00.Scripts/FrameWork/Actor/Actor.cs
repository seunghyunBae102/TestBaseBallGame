
using System.Collections.Generic;
using UnityEngine;

public class Actor : GetCompoParentSample<Actor>, IGetCompoable<ActorManager>
{
    protected ActorManager _mom;
    public ActorManager Mom
    {
        get { return _mom; }
        protected set { _mom = value; }
    }
    protected Actor _dad; // Mom is Component's Parent(Actor's mom => ActorManager) so ParentActor is Dad HEHEHEHA
    public Actor Dad
    {
        get { return _dad; }
        protected set { _dad = value; }
    }
    protected string _name;
    public string Name
    {
        get { return _name; }
        protected set { _name = value; }
    }

    protected void Start()
    {
        if(_mom == null)
        {
            Transform trm = transform;
            while(true)
            {
                if(trm.gameObject.TryGetComponent<ActorManager>(out var mom))
                {
                    _mom = mom;
                    _mom.AddCompoDic(this.GetType(), this);
                    break;
                }
                if (trm.parent == null)
                    break;
            }
        }
    }

    protected List<Actor> _childActors = new List<Actor>();

    public override void Init()
    {
        
    }

    public void AddChild()
    {
        _childActors.Add(this);
    }
    public Actor GetChild(int idx)
    {
        return _childActors[idx];
    }
    public Actor GetChild(string name)
    {
        foreach (var child in _childActors)
        {
            if (child.Name == name)
                return child;
        }
        return null;
    }
    public List<Actor> GetAllChildren()
    {
        return _childActors;
    }

    public K GetCompoInChilndren<K> (string name) where K : IGetCompoable<Actor>
    {
        foreach (var child in _childActors)
        {
            K compo = GetCompo<K>(name);
            if(compo != null)
                return compo;
            return child.GetCompoInChilndren<K>(name);
        }
        return default;
    }
    public K GetCompoInChilndren<K>() where K : IGetCompoable<Actor>
    {
        foreach (var child in _childActors)
        {
            K compo = GetCompo<K>();
            if (compo != null)
                return compo;
            return child.GetCompoInChilndren<K>();
        }
        return default;
    }

    public List<K> GetComposInChilndren<K>(string name) where K : IGetCompoable<Actor>
    {
        List<K> comps = new List<K>();
        foreach (var child in _childActors)
        {
            K compo = GetCompo<K>(name);
            if (compo != null)
                comps.Add(compo);
            comps.AddRange(child.GetComposInChilndren<K>(name));
        }
        return comps;
        // looks slow HEHEHEHA like Scat Code
    }
    public List<K> GetComposInChilndren<K>() where K : IGetCompoable<Actor>
    {
        List<K> comps = new List<K>();
        foreach (var child in _childActors)
        {
            K compo = GetCompo<K>();
            if (compo != null)
                comps.Add(compo);
            comps.AddRange(child.GetComposInChilndren<K>());
        }
        return comps;
    }

    public Actor GetDad()
    {
        return _dad;
    }

    public void SetDad(Actor dad)
    {
        _dad.RemoveChild(this);
        _dad = dad;
    }

    public int GetChildCount()
    {
        return _childActors.Count;
    }

    public bool HasChild(string name)
    {
        foreach (var child in _childActors)
        {
            if (child.Name == name)
                return true;
        }
        return false;
    }
    public bool HasChild(int idx)
    {
        return idx >= 0 && idx < _childActors.Count;
    }
    public bool HasChild(Actor actor)
    {
        return _childActors.Contains(actor);
    }

    public void RemoveChild(int idx)
    {
        _childActors.RemoveAt(idx);
    }
    public void RemoveChild(string name)
    {
        foreach (var child in _childActors)
        {
            if (child.Name == name)
            {
                _childActors.Remove(child);
                break;
            }
        }
    }
    public void RemoveChild(Actor actor)
    {
        _childActors.Remove(actor);
    }

    public void RegisterEvents(ActorManager main)
    {
        Mom = main;
    }

    public void UnregisterEvents(ActorManager main)
    {
        _mom.RemoveCompoDic<Actor>(this.Name);
    }
}
