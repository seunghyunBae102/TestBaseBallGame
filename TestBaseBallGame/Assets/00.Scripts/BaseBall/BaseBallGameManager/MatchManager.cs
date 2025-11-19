// BaseBall/Match/MatchManager.cs (새로 추가)
public class MatchManager
    : BaseBallGetableManager,                   // 이미 있는 패턴 활용
      ILifeCycleable<BaseBallGameManager>,      // BaseBallGameManager 라이프사이클
       IGameEventListener<ScoreChangedEvent>       // 예시: 나중에 실제 이벤트 타입으로 교체
{
    public BaseBallGameManager Mom { get; private set; }

    // 나중에 채울 것들
    public MatchContext Context { get; private set; }

    #region IGetCompoable<BaseBallGameManager>

    // BaseBallGameManager.Init() -> GetCompoParentSample<BaseBallGameManager>.Init() 에서 호출됨
    public override void Init(BaseBallGameManager mom)
    {
        Mom = mom;
        base.Init(mom); // BaseBallGetableManager에 있는 기본 Init (AddCompoDic 등)

        // 여기서 MatchContext 초기화 or 필드/팀 세팅 가능
        Context = new MatchContext();
    }
    public void AddRun(int teamIndex)
    {
        var oldScore = Context.Scores[teamIndex];
        //var newScore = oldScore + 1;
        //Context.Scores[teamIndex] = newScore;

        // 이벤트 발행
        Mom.Mom.EventBus.Publish(new ScoreChangedEvent
        {
            TeamIndex = teamIndex,
            //OldScore = oldScore,
            //NewScore = newScore
        });
    }
    public override void RegisterEvents(BaseBallGameManager main)
    {
        // Game 전체 EventBus를 쓰고 싶으면:
        // main.Mom.EventBus.Subscribe(this);

        // 또는 야구 전용 버스가 있다면 main.EventBus.Subscribe(this); 등
    }

    public override void UnregisterEvents(BaseBallGameManager main)
    {
        // main.Mom.EventBus.Unsubscribe(this);
    }

    #endregion

    #region ILifeCycleable<BaseBallGameManager>

    // BaseBallGameManager.Update() -> GetCompoParentSample<BaseBallGameManager>.Update()에서 호출
    public void Tick(float dt)
    {
        // 매 프레임 경기 진행 (카운트/상태머신 등)
    }

    public void TickFixed(float fdt)
    {
        // Fixed 진행이 필요하면 (물리 기반 주루/수비 등)
    }

    public void AfterInit()
    {
        // 모든 매니저/컴포넌트 Init 끝난 후 한 번만
        // ex) 초기 이닝 세팅, 첫 타자 세팅 등
    }

    public void BeforeDestroy()
    {
        // 경기 종료/씬 전환 직전 정리
    }

    #endregion

    #region IGameEventListener<SomeMatchEvent>

    public void OnEvent(ScoreChangedEvent e)
    {
        // 나중에 이벤트 버스 설계할 때 실제 이벤트 타입으로 바꿔서 구현
    }

    #endregion
}
