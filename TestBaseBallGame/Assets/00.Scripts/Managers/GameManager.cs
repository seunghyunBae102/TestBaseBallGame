using UnityEngine;

public class GameManager : GetCompoParentSample<GameManager>
{
    private static GameManager _instance;



public static GameManager GetGameManager()
    {
        if(_instance == null)
        {
           _instance = new GameObject("GameManager", typeof(GameManager)).GetComponent<GameManager>();
        }
        else
        {
            
        }

            return _instance;
    }

    protected override void Awake()
    {
        Init();
        base.Awake();
    }

    public void Init()
    {
        if( _instance == null )
        {
            _instance = this;

        }
        else
        {
           
        }
    }

    public override T GetCompo<T>(bool isIncludeChild = false)
    {
        if (base.GetCompo<T>(isIncludeChild) == null)
        {
            AddRealCompo<T>();
        }
        return base.GetCompo<T>(isIncludeChild);

        //Create COmpo when No Compo HEHEHA
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