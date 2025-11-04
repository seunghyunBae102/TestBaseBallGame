using UnityEngine;

//public class BaseBallGetableGetableComponent : BaseGameCompo, IGetCompoable<BaseBallGetableParentManager>
//{
//    [HideInInspector]
//    public BaseBallGetableParentManager Mom;

//    protected void Awake()
//    {
//        //컴포넌트가 알아서 붙는
//        if (Mom == null)
//        {
//            Transform trm = GetComponent<Transform>();

//            while (trm.parent != null && trm.gameObject.GetComponent<Actor>())
//            {
//                trm = trm.parent;
//            }
//            Init(trm.GetComponent<BaseBallGetableParentManager>());
//        }
//    }

//    public virtual void Init(BaseBallGetableParentManager mom)
//    {
//        Mom = mom;
//        mom.AddCompoDic(this.GetType(), this);
//    }
//    public void RegisterEvents(BaseBallGetableParentManager main)
//    {
//        throw new System.NotImplementedException();
//    }

//    public void UnregisterEvents(BaseBallGetableParentManager main)
//    {
//        throw new System.NotImplementedException();
//    }
//}

