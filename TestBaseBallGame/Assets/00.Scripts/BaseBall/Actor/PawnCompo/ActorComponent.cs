using UnityEngine;

public class ActorComponent : BaseGameCompo, IGetCompoable<Actor>
{
    public void Init(Actor mom)
    {
        mom.AddCompoDic(this.GetType(), this);
    }

    public void RegisterEvents(Actor main)
    {
        throw new System.NotImplementedException();
    }

    public void UnregisterEvents(Actor main)
    {
        throw new System.NotImplementedException();
    }
}
