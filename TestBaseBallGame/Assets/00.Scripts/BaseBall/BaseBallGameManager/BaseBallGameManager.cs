using UnityEngine;

public class BaseBallGameManager : GetCompoParentSample<BaseBallGameManager>, IGetCompoable<GameManager>
{
    [HideInInspector]
    public GameManager Mom;
    public BaseBallRunningManager RunningManager { get; private set; }
    public BaseBallRaycastManager RaycastManager { get; private set; }
    public BaseBallGetableManager GetableManager { get; private set; }

    private BaseBallRule _gameRule;

    [SerializeField]
    private Transform[] _bases; // 1루, 2루, 3루, 홈

    private int _currentInning = 1;
    private bool _isTopInning = true;

    private void Start()
    {
        if (Mom == null)
        {

            if (GameManager.Instance != null)
                Init(GameManager.Instance);

        }
        RunningManager = GetComponent<BaseBallRunningManager>();
        RaycastManager = GetComponent<BaseBallRaycastManager>();
        GetableManager = GetComponent<BaseBallGetableManager>();
        _gameRule = GetComponent<BaseBallRule>();
    }

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

    public void StartNewInning()
    {
        if (!_isTopInning)
        {
            _currentInning++;
        }
        _isTopInning = !_isTopInning;

        // 이닝 시작 준비
        PrepareBatterAndPitcher();
    }

    private void PrepareBatterAndPitcher()
    {
        // 타자와 투수 위치 세팅
        // 각 팀의 다음 타자 선정
    }

    public Transform GetBaseTransform(int baseNumber)
    {
        return _bases[Mathf.Clamp(baseNumber, 0, 3)];
    }

    public void ProcessHit(Vector3 ballPosition, float power)
    {
        // 안타 판정 및 주자 진루 처리
        RunningManager.ProcessRunners(ballPosition);
    }

}
