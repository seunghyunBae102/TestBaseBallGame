using UnityEngine;

public class BaseBallGetableManager : BaseGameCompo, IGetCompoable<BaseBallGameManager>
{
    [HideInInspector]
    public BaseBallGameManager Mom;

    public virtual void Init(BaseBallGameManager mom)
    {
        mom.AddCompoDic(this.GetType(), this);
        Mom = mom;
    }

    public void RegisterEvents(BaseBallGameManager main)
    {
        throw new System.NotImplementedException();
    }

    public void UnregisterEvents(BaseBallGameManager main)
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
                if (GameManager.Instance.GetCompo<BaseBallGameManager>() != null)
                    Init(GameManager.Instance.GetCompo<BaseBallGameManager>());

        }
    }
}
