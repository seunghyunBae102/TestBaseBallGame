using UnityEngine;

public class GetableManager : BaseGameCompo,IGetCompoable<GameManager>
{
    [HideInInspector]
    public GameManager Mom;

    public virtual void Init(GameManager mom)
    {
        mom.AddCompoDic(this.GetType(), this);
        Mom = mom;
    }

    public void RegisterEvents(GameManager main)
    {
        //if (this is IGameEventListener<SomeEvent> listener)
        //    main.EventBus.Subscribe(listener);
    }

    public virtual void UnregisterEvents(GameManager main)
    {
        //if (this is IGameEventListener<SomeEvent> listener)
        //    main.EventBus.Unsubscribe(listener);
    }
    private void Start()
    {
        if(Mom == null)
        {
            //Transform trm = GetComponent<Transform>();

            //while(trm.parent != null && trm.gameObject.GetComponent<GameManager>())
            //{
            //    trm = trm.parent;
            //}
            //Init(trm.GetComponent<GameManager>());
            if (GameManager.Instance != null)
                Init(GameManager.Instance);

        }
    }
}

public class GetableManagerParent<T> : GetCompoParentSample<T>, IGetCompoable<GameManager> where T : IGetCompoParent<T>
{
    [HideInInspector]
    public GameManager Mom;

    public virtual void Init(GameManager mom)
    {
        mom.AddCompoDic(this.GetType(), this);
        Mom = mom;
    }

    public void RegisterEvents(GameManager main)
    {
        throw new System.NotImplementedException();
    }

    public void UnregisterEvents(GameManager main)
    {
        throw new System.NotImplementedException();
    }

    private void Start()
    {
        if (Mom == null)
        {
            //Transform trm = GetComponent<Transform>();

            //while(trm.parent != null && trm.gameObject.GetComponent<GameManager>())
            //{
            //    trm = trm.parent;
            //}
            //Init(trm.GetComponent<GameManager>());
            if (GameManager.Instance != null)
                Init(GameManager.Instance);

        }
    }
}
