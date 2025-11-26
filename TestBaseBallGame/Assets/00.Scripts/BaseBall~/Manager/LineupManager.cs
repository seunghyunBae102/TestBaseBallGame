// Scripts/Game/Baseball/Match/LineupManager.cs
using UnityEngine;


    /// <summary>
    /// 홈/원정 팀 라인업과 현재 타자/투수를 관리한다.
    /// - InningHalfStarted 이벤트를 보고 공격팀을 결정
    /// - AtBatEnded 이벤트를 보고 다음 타자를 순환
    /// - AtBatStarted 이벤트를 발행
    /// </summary>
    public class LineupManager : ManagerBase
    {
        [Header("팀 데이터")]
        [SerializeField] private TeamDataSO _homeTeam;
        [SerializeField] private TeamDataSO _awayTeam;

        private int _homeBatterIndex;
        private int _awayBatterIndex;

        private PlayerDataSO _homePitcher;
        private PlayerDataSO _awayPitcher;

        protected override void Initialize()
        {
            base.Initialize();

            // 초기값 설정
            _homePitcher = _homeTeam?.startingPitcher;
            _awayPitcher = _awayTeam?.startingPitcher;

            _homeBatterIndex = 0;
            _awayBatterIndex = 0;

            Events?.Subscribe<InningHalfStarted>(OnInningHalfStarted);
            Events?.Subscribe<AtBatEnded>(OnAtBatEnded);
        }

        protected override void OnDestroy()
        {
            Events?.Unsubscribe<InningHalfStarted>(OnInningHalfStarted);
            Events?.Unsubscribe<AtBatEnded>(OnAtBatEnded);

            base.OnDestroy();
        }

        private void OnInningHalfStarted(InningHalfStarted e)
        {
            // 새로운 공격 이닝이 시작될 때 자동으로 타석 시작
            StartNewAtBat(e.Offense);
        }

        private void OnAtBatEnded(AtBatEnded e)
        {
            // 다음 타자 index로 증가
            if (e.Offense == TeamSide.Home)
            {
                _homeBatterIndex = GetNextIndex(_homeTeam.battingOrder, _homeBatterIndex);
            }
            else
            {
                _awayBatterIndex = GetNextIndex(_awayTeam.battingOrder, _awayBatterIndex);
            }

            // 다음 타석 시작
            StartNewAtBat(e.Offense);
        }

        private int GetNextIndex(PlayerDataSO[] order, int current)
        {
            if (order == null || order.Length == 0) return 0;
            int next = current + 1;
            if (next >= order.Length) next = 0;
            return next;
        }

        private void StartNewAtBat(TeamSide offense)
        {
            TeamDataSO offenseTeam = offense == TeamSide.Home ? _homeTeam : _awayTeam;
            TeamDataSO defenseTeam = offense == TeamSide.Home ? _awayTeam : _homeTeam;

            int batterIndex = offense == TeamSide.Home ? _homeBatterIndex : _awayBatterIndex;

            var batter = GetBatter(offenseTeam, batterIndex);
            var pitcher = GetPitcher(defenseTeam, offense);

            if (batter == null || pitcher == null)
            {
                Debug.LogWarning("LineupManager: Batter or Pitcher is null – 팀 데이터 설정 확인 필요");
                return;
            }

            Events?.Publish(new AtBatStarted
            {
                Offense = offense,
                BatterIndex = batterIndex,
                Batter = batter,
                Pitcher = pitcher
            });
        }

        private PlayerDataSO GetBatter(TeamDataSO team, int batterIndex)
        {
            if (team == null || team.battingOrder == null || team.battingOrder.Length == 0)
                return null;

            if (batterIndex < 0 || batterIndex >= team.battingOrder.Length)
                batterIndex = 0;

            return team.battingOrder[batterIndex];
        }

        private PlayerDataSO GetPitcher(TeamDataSO team, TeamSide defendingSide)
        {
            // 일단 선발 투수만 사용. 나중에 불펜/교체 로직 추가
            return team?.startingPitcher;
        }
    }


