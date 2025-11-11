
using System.Collections.Generic;
using UnityEngine;

public class Actor : GameObjV2<Actor>, IGetCompoable<ActorManager>
{
    protected ActorManager _mom;
    public ActorManager Mom
    {
        get { return _mom; }
        protected set { _mom = value; }
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
