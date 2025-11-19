// BaseBall/Manager/BaseBallGameManager.cs (이름 그대로 써도 됨)
public class BaseBallGameManager
    : GetCompoParentSample<BaseBallGameManager>,   // 자기 애들을 관리
      IGetCompoable<GameManager>,                  // GameManager의 자식
      ILifeCycleable<GameManager>                  // GameManager 라이프사이클에 참여
{
    public GameManager Mom { get; private set; }

    // 야구 도메인의 서브 시스템들
    public MatchManager MatchManager { get; private set; }
    public BaseBallFieldManager FieldManager { get; private set; }
    public BaseBallRaycastManager RaycastManager { get; private set; }

    #region IGetCompoable<GameManager>

    // GameManager.Init()에서 자동으로 호출됨
    public void Init(GameManager mom)
    {
        Mom = mom;
        // GameManager 입장에서 BaseBallGameManager 등록
        mom.AddCompoDic(this.GetType(), this);

        // 자기 자신(BaseBallGameManager)도 초기화 시작
        // GetCompoParentSample<BaseBallGameManager>.Init()을 한번 더 돌려야 함
        base.Init(); // 이 안에서 자기 자식(IGetCompoable<BaseBallGameManager>) 들 Init/AfterInit/이벤트 등록
    }

    public void RegisterEvents(GameManager main)
    {
        // 필요하면 GameManager의 EventBus에 구독도 가능
        // main.EventBus.Subscribe(...);
    }

    public void UnregisterEvents(GameManager main)
    {
        // main.EventBus.Unsubscribe(...);
    }

    #endregion

    #region ILifeCycleable<GameManager>

    // GameManager.Update()에서 호출됨
    public void Tick(float dt)
    {
        // 야구 쪽 전역 Tick 필요하면 여기
        // 나머지는 GetCompoParentSample<BaseBallGameManager>의 Update()를 통해
        // MatchManager/FieldManager 등으로 흘러감
    }

    public void TickFixed(float fdt) { }
    public void AfterInit() { }
    public void BeforeDestroy() { }

    #endregion

    protected override void Awake()
    {
        // GameManager가 Init()으로 전체를 제어하니까,
        // BaseBallGameManager에서 직접 Init()를 호출할 필요는 없음.
        // base.Awake(); 호출 여부는 네 프레임워크 구현에 맞춰서.
        base.Awake();
    }

    // BaseBallGameManager 자신의 Init(부모 없음 버전)을 Override 하고 싶으면 여기서
    public override void Init()
    {
        // 1) 자식 서브 매니저 캐싱
        MatchManager = GetComponentInChildren<MatchManager>(true);
        FieldManager = GetComponentInChildren<BaseBallFieldManager>(true);
        RaycastManager = GetComponentInChildren<BaseBallRaycastManager>(true);

        // 2) 기본 Init 흐름 유지
        base.Init();
    }
    public void StartNewInning()
    {
        //MatchManager.StartNewInning();
    }
}


//using UnityEngine;

//public class BaseBallGameManager : GetCompoParentSample<BaseBallGameManager>, IGetCompoable<GameManager>
//{
//    [HideInInspector]
//    public GameManager Mom;
//    public BaseBallRunningManager RunningManager { get; private set; }
//    public BaseBallRaycastManager RaycastManager { get; private set; }
//    public BaseBallGetableManager GetableManager { get; private set; }

//    private BaseBallRule _gameRule;

//    [SerializeField]
//    private Transform[] _bases; // 1루, 2루, 3루, 홈

//    private int _currentInning = 1;
//    private bool _isTopInning = true;

//    private void Start()
//    {
//        if (Mom == null)
//        {

//            if (GameManager.Instance != null)
//                Init(GameManager.Instance);

//        }
//        RunningManager = GetComponent<BaseBallRunningManager>();
//        RaycastManager = GetComponent<BaseBallRaycastManager>();
//        GetableManager = GetComponent<BaseBallGetableManager>();
//        _gameRule = GetComponent<BaseBallRule>();
//    }

//    public virtual void Init(GameManager mom)
//    {
//        mom.AddCompoDic(this.GetType(), this);
//        Mom = mom;
//    }

//    public void RegisterEvents(GameManager main)
//    {
//        throw new System.NotImplementedException();
//    }

//    public void UnregisterEvents(GameManager main)
//    {
//        throw new System.NotImplementedException();
//    }

//    public void StartNewInning()
//    {
//        if (!_isTopInning)
//        {
//            _currentInning++;
//        }
//        _isTopInning = !_isTopInning;

//        // 이닝 시작 준비
//        PrepareBatterAndPitcher();
//    }

//    private void PrepareBatterAndPitcher()
//    {
//        // 타자와 투수 위치 세팅
//        // 각 팀의 다음 타자 선정
//    }

//    public Transform GetBaseTransform(int baseNumber)
//    {
//        return _bases[Mathf.Clamp(baseNumber, 0, 3)];
//    }

//    public void ProcessHit(Vector3 ballPosition, float power)
//    {
//        // 안타 판정 및 주자 진루 처리
//        RunningManager.ProcessRunners(ballPosition);
//    }

//}
