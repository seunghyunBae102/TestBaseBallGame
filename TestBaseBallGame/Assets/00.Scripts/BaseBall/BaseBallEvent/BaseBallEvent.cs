using UnityEngine;
using static BaseBallRule;

public class BaseBallEvent
{ }

public class MatchContext
{
    public int CurrentInning;
    public bool IsTop;           // 초/말
//    public CountState Count;     // Balls, Strikes, Outs
    public ScoreInfo[] Scores;   // 기존 BaseBallRule.ScoreInfo 재사용
//    public BaseState Bases;      // 1,2,3루 주자
    //public TeamIndex AtBatTeam;  // 공격팀 인덱스
    //public TeamIndex DefenseTeam;
    // 나중에 필요하면: PitchState, WeatherState 등 추가
}

public interface IGameEvent { }

public struct ScoreChangedEvent : IGameEvent
{
    public int TeamIndex;     // 0: 홈, 1: 원정 이런 식
    public int OldScore;
    public int NewScore;
}
