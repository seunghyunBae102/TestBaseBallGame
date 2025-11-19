using System;

public interface IGetCompoParent<T> where T : IGetCompoParent<T>
{
    GameEventBus EventBus { get; }
    void Init();
    void RegisterEvents();
    void UnregisterEvents();

    public void AddCompoDic(System.Type type, IGetCompoable<T> compo);
    public void RemoveCompoDic<K>() where K : IGetCompoable<T>;
    public void ClearCompoDic();

    public K GetCompo<K>(string name = "", bool isIncludeChild = false) where K : IGetCompoable<T>;
    public K GetCompo<K>(bool isIncludeChild = false) where K : IGetCompoable<T>;
    //public U GetCompo<U>() where U : IGetCompoable<T>;
    public bool HasCompo(Type type);
    public bool HasCompo(Type type, string name);
    public bool HasCompo(string name);
    public int CompoCount();
    public void ForEachCompo(System.Action<IGetCompoable<T>> action);
    public void ForEachCompo<U>(System.Action<U> action) where U : IGetCompoable<T>;
}
