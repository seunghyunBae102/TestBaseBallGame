using UnityEngine;

public class ActorComponent : GetCompoParentSample<ActorComponent>, IGetCompoable<Actor>
{
    [HideInInspector]
    public Actor Mom;

    protected virtual void Awake()
    {
        //컴포넌트가 알아서 붙는
        if(Mom == null)
        {
            Transform trm = GetComponent<Transform>();
            Actor actor = null;
            while (trm.parent != null && !trm.gameObject.TryGetComponent<Actor>(out actor))
            {
                trm = trm.parent;
            }
            if(actor)
            Init(actor);
        }
    }

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
