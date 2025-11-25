using System;

public interface IGetCompoParent<T> where T : IGetCompoParent<T>
{
    GameEventBus EventBus { get; }
    void Init();
    void RegisterEvents();
    void UnregisterEvents();

    public void AddCompoDic(System.Type type, IGetCompoable<T> compo);
    public void RemoveCompoDic(System.Type type);
    public void ClearCompoDic();

    public IGetCompoable<T> GetCompo(System.Type type);
    public U GetCompo<U>() where U : IGetCompoable<T>;
    public bool HasCompo(Type type);
    public bool HasCompo(Type type, string name);
    public bool HasCompo(string name);
    public int CompoCount();
    public void ForEachCompo(System.Action<IGetCompoable<T>> action);
    public void ForEachCompo<U>(System.Action<U> action) where U : IGetCompoable<T>;
}
