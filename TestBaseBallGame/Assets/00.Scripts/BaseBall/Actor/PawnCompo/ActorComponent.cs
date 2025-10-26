using UnityEngine;

public class ActorComponent : BaseGameCompo, IGetCompoable<Actor>
{
    [HideInInspector]
    public Actor Mom;
    public virtual void Init(Actor mom)
    {
        Mom = mom;
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
