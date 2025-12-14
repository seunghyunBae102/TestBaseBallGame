using UnityEngine;
using Bash.Core.Ball;
using Bash.Core.GamePlay;
using Bash.Framework.Managers; // EMatchState 참조용

namespace Bash.Framework.Core.Events
{
    // =================================================================================
    // [Section 1] Game Flow Events (게임 흐름 및 상태)
    // =================================================================================

    // [Fix] 누락되었던 라운드(타석) 시작 이벤트
    // 용도: FieldingDirector가 수비 위치를 리셋하거나, UI가 타자 정보를 갱신할 때 사용
    public struct RoundStartEvent { }

    // 경기 상태(FSM) 변경 알림
    // 용도: UI(점수판)나 카메라 연출이 상태 변화(투구 대기 -> 인플레이)를 감지할 때 사용
    public struct MatchStateChangeEvent
    {
        public EMatchState NewState;
    }

    // =================================================================================
    // [Section 2] Action Events (투구, 타격, 수비)
    // =================================================================================

    // 투수 투구 시작
    public struct PitchStartEvent { }

    // 타격 발생 (공이 배트에 맞음)
    public struct BallHitEvent
    {
        public Vector3 LandingPoint; // 낙구 예상 지점
        public bool IsFlyBall;       // true: 뜬공, false: 땅볼/라인드라이브
    }

    // 수비수 포구 (공을 잡음)
    public struct BallCaughtEvent
    {
        public FielderPawn Catcher; // 누가 잡았는지
    }

    // 공이 지면에 착지 (혹은 펜스 넘어감)
    public struct BallHitGroundEvent
    {
        public Vector3 LandingPosition;
    }

    // =================================================================================
    // [Section 3] Judgement & Rules (판정 및 규칙)
    // =================================================================================

    // 판정 요청 (투구 결과)
    // 발생: 공이 포수 미트에 도달했을 때 (Pitcher/Catcher)
    public struct BallReachCatcherEvent
    {
        public bool IsStrikeZone; // 스트라이크 존 통과 여부
        public bool IsSwung;      // 타자가 배트를 휘둘렀는가
    }

    // 예비 판정 (스킬 개입용 Hook)
    // 발생: GameRuleManager가 판정을 내리기 직전
    public class PreJudgementEvent
    {
        public JudgementEvent.Type OriginalResult; // 원래 판정
        public JudgementEvent.Type FinalResult;    // 변경될 판정
        public bool IsModified = false;            // 스킬 개입 여부
    }

    // 최종 판정 결과
    // 발생: GameRuleManager (UI 갱신용)
    public struct JudgementEvent
    {
        public enum Type { Strike, Ball, Foul, Hit, Out, HomeRun }
        public Type Result;
        public string Message; // "STRIKE!", "OUT!" 등 텍스트
    }

    // 게임 스코어 및 카운트 상태 변경
    public struct GameStateEvent
    {
        public int Score;
        public int Strike;
        public int Ball;
        public int Out;
    }
}