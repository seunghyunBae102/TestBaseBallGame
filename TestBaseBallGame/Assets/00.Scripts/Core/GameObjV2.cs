using System.Collections.Generic;
using UnityEngine;

public class GameObjV2<T> : GetCompoParentSample<T> where T: GameObjV2<T>
{

    protected T _dad; // Mom is Component's Parent(GameObjV2's mom => GameObjV2Manager) so ParentGameObjV2 is Dad HEHEHEHA
    public T Dad
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

    protected List<T> _childGameObjV2s = new List<T>();

    public override void Init()
    {

    }

    public void AddChild(T child)
    {
        if (child != null)
        {
            _childGameObjV2s.Add(child);
        }
    }
    public T GetChild(int idx)
    {
        return _childGameObjV2s[idx];
    }
    public T GetChild(string name)
    {
        foreach (var child in _childGameObjV2s)
        {
            if (child.Name == name)
                return child;
        }
        return null;
    }
    public List<T> GetAllChildren()
    {
        return _childGameObjV2s;
    }

    public K GetCompoInChilndren<K>(string name) where K : IGetCompoable<T>
    {
        foreach (var child in _childGameObjV2s)
        {
            K compo = GetCompo<K>(name);
            if (compo != null)
                return compo;
            return child.GetCompoInChilndren<K>(name);
        }
        return default;
    }
    public K GetCompoInChilndren<K>() where K : IGetCompoable<T>
    {
        foreach (var child in _childGameObjV2s)
        {
            K compo = GetCompo<K>();
            if (compo != null)
                return compo;
            return child.GetCompoInChilndren<K>();
        }
        return default;
    }

    public List<K> GetComposInChilndren<K>(string name) where K : IGetCompoable<T>
    {
        List<K> comps = new List<K>();
        foreach (var child in _childGameObjV2s)
        {
            K compo = GetCompo<K>(name);
            if (compo != null)
                comps.Add(compo);
            comps.AddRange(child.GetComposInChilndren<K>(name));
        }
        return comps;
        // looks slow HEHEHEHA like Scat Code
    }
    public List<K> GetComposInChilndren<K>() where K : IGetCompoable<T>
    {
        List<K> comps = new List<K>();
        foreach (var child in _childGameObjV2s)
        {
            K compo = GetCompo<K>();
            if (compo != null)
                comps.Add(compo);
            comps.AddRange(child.GetComposInChilndren<K>());
        }
        return comps;

    }

    public T GetDad()
    {
        return _dad;
    }

    public void SetDad(T dad)
    {
        _dad.RemoveChild((T)this);
        _dad = dad;
    }

    public int GetChildCount()
    {
        return _childGameObjV2s.Count;
    }

    public bool HasChild(string name)
    {
        foreach (var child in _childGameObjV2s)
        {
            if (child.Name == name)
                return true;
        }
        return false;
    }
    public bool HasChild(int idx)
    {
        return idx >= 0 && idx < _childGameObjV2s.Count;
    }
    public bool HasChild(T GameObjV2)
    {
        return _childGameObjV2s.Contains(GameObjV2);
    }

    public void RemoveChild(int idx)
    {
        _childGameObjV2s.RemoveAt(idx);
    }
    public void RemoveChild(string name)
    {
        foreach (var child in _childGameObjV2s)
        {
            if (child.Name == name)
            {
                _childGameObjV2s.Remove(child);
                break;
            }
        }
    }
    public void RemoveChild(T GameObjV2)
    {
        _childGameObjV2s.Remove(GameObjV2);
    }

}
