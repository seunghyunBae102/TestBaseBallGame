using UnityEngine;

public interface IGetCompoable<T> where T : IGetCompoParent<T>
{
    public void Init(T mom)
    {
        mom.AddCompoDic(this.GetType(),this);
    }
    void RegisterEvents(T main);
    void UnregisterEvents(T main);
}
