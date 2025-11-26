using UnityEngine;

/// <summary>
/// "현재 타석"의 시작/종료를 집계하는 매니저.
/// - AtBatStarted가 들어오면 현재 타석 정보 저장
/// - BatterOut / BatterAwardedBase 등 타석 종료 이벤트를 모아 AtBatEnded 발행
/// </summary>
public class AtBatManager : ManagerBase
{
    private bool _hasActiveAtBat;
    private TeamSide _offense;
    private int _batterIndex;

    private bool _ballInPlay;
    private bool _walk;
    private bool _strikeOut;
    private bool _hitByPitch;

    protected override void Initialize()
    {
        base.Initialize();

        Events?.Subscribe<AtBatStarted>(OnAtBatStarted);
        Events?.Subscribe<BatterOut>(OnBatterOut);
        Events?.Subscribe<BatterAwardedBase>(OnBatterAwardedBase);
    }

    protected override void OnDestroy()
    {
        Events?.Unsubscribe<AtBatStarted>(OnAtBatStarted);
        Events?.Unsubscribe<BatterOut>(OnBatterOut);
        Events?.Unsubscribe<BatterAwardedBase>(OnBatterAwardedBase);

        base.OnDestroy();
    }

    private void OnAtBatStarted(AtBatStarted e)
    {
        _hasActiveAtBat = true;
        _offense = e.Offense;
        _batterIndex = e.BatterIndex;

        _ballInPlay = false;
        _walk = false;
        _strikeOut = false;
        _hitByPitch = false;

        Debug.Log($"AtBatManager: 타석 시작 – {e.Offense} 팀 {e.BatterIndex + 1}번 {e.Batter.displayName}");
    }

    private void OnBatterOut(BatterOut e)
    {
        if (!_hasActiveAtBat) return;

        // 삼진/플라이 아웃 여부는 Reason 문자열을 보고 최소한으로 구분
        if (e.Reason.Contains("Strike"))
            _strikeOut = true;

        EndAtBat();
    }

    private void OnBatterAwardedBase(BatterAwardedBase e)
    {
        if (!_hasActiveAtBat) return;

        if (e.IsWalk) _walk = true;
        if (e.IsHitByPitch) _hitByPitch = true;
        if (e.IsHit) _ballInPlay = true; // 인플레이 안타로 간주

        EndAtBat();
    }

    private void EndAtBat()
    {
        if (!_hasActiveAtBat) return;
        _hasActiveAtBat = false;

        Events?.Publish(new AtBatEnded
        {
            Offense = _offense,
            BatterIndex = _batterIndex,
            PutBallInPlay = _ballInPlay,
            Walk = _walk,
            StrikeOut = _strikeOut,
            HitByPitch = _hitByPitch
        });

        Debug.Log($"AtBatManager: 타석 종료 – Walk:{_walk}, SO:{_strikeOut}, HBP:{_hitByPitch}, BIP:{_ballInPlay}");
    }
}
