using UnityEngine;

public class GameManager : GetCompoParentSample<GameManager>
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("GameManager", typeof(GameManager)).GetComponent<GameManager>();
            }
            else
            {

            }

            return _instance;
        }
    }

    protected override void Awake()
    {
        Init();
        base.Awake();
    }

    public override void Init()
    {
        if( _instance == null )
        {
            _instance = this;

        }
        else
        {
           
        }
        base.Init();
    }

    public override K GetCompo<K>(bool isIncludeChild = false)
    {

        //if (base.GetCompo<K>(isIncludeChild) == null)
        //{
        //    if (typeof(UnityEngine.Component).IsAssignableFrom(typeof(K)))
        //        AddRealCompo<K>("");
        //}
        //return base.GetCompo<K>(isIncludeChild);
        Debug.Log("FIXHERE)");


        return base.GetCompo<K>(isIncludeChild);


        //Create Compo when No Compo HEHEHA
    }
}

    //public static GameManager Instance
    //{
    //    get
    //    {
    //        
    //if(Instance == null)
    //    {
    //       Instance = new GameObject("GameManager", typeof(GameManager)).GetComponent<GameManager>();
    //    }

//        return Instance;
//    }

//    private set;
//}