// Scripts/Game/Baseball/Core/BaseballSharedAndEvents.cs
using Bash.Framework.Core;

namespace Bash.Game.Baseball.Shared
{
    /// <summary>
    /// 양 팀 구분용 enum.
    /// </summary>
    public enum TeamSide
    {
        Home = 0,
        Away = 1
    }
}

namespace Bash.Game.Baseball.Events
{
    using Bash.Game.Baseball.Shared;

    #region Pitch / Hit 결과 타입

    /// <summary>
    /// 투구 결과(룰에서 해석된 최종 결과).
    /// RawPitchResult의 bool 조합을 해석해서 이 타입으로 만든다.
    /// </summary>
    public enum PitchOutcomeType
    {
        Ball,
        CalledStrike,   // 스윙 안 하고 존 안
        SwingingStrike, // 스윙했지만 헛스윙
        Foul,
        InPlayFair,
        InPlayFoul,
        HitByPitch
    }

    /// <summary>
    /// Rule_Pitch 모듈이 발행하는 "투구 결과".
    /// </summary>
    public struct PitchOutcome
    {
        public PitchOutcomeType Type;
    }

    /// <summary>
    /// 타구 타입.
    /// </summary>
    public enum HitType
    {
        GroundBall,
        LineDrive,
        FlyBall,
        FoulBall,
        HomeRun
    }

    /// <summary>
    /// 타구 결과.
    /// - 타구 모듈(HitBallModule) → 룰 모듈(Rule_HitResultModule)로 전달.
    /// </summary>
    public struct HitBallResult
    {
        /// <summary>타구 종류.</summary>
        public HitType HitType;

        /// <summary>페어인지 여부.</summary>
        public bool IsFair;

        /// <summary>공중에서 잡혀 아웃이 되었는지 여부.</summary>
        public bool IsCaughtInAir;

        /// <summary>
        /// 타자에게 부여되는 기본 베이스 수.
        /// - 1: 단타, 2: 2루타, 3: 3루타, 4: 홈런(또는 인사이드 더 파크)
        /// </summary>
        public int BaseCount;

        /// <summary>홈런인지 여부.</summary>
        public bool IsHomeRun;
    }

    #endregion

    #region Raw 입력 (물리/판정 → 룰)

    /// <summary>
    /// 투구 결과의 "로우 데이터".
    /// - 피치 모듈 → 룰 모듈(Rule_PitchModule)로 전달되는 정보
    /// - 이 레벨은 물리/충돌/존 판단 등 low-level 정보만 담는다.
    /// </summary>
    public struct RawPitchResult
    {
        /// <summary>스트라이크 존 안에 들어갔는지 여부.</summary>
        public bool IsInStrikeZone;

        /// <summary>타자가 스윙했는지 여부.</summary>
        public bool BatterSwung;

        /// <summary>번트 시도인지 여부.</summary>
        public bool IsBuntAttempt;

        /// <summary>배트와 공이 실제로 접촉했는지.</summary>
        public bool Contact;

        /// <summary>인플레이 상태의 페어 타구인지.</summary>
        public bool BallInPlayFair;

        /// <summary>파울 영역으로 나간 공인지.</summary>
        public bool BallInFoul;

        /// <summary>파울팁이 포수에게 잡힌 경우(삼진 가능).</summary>
        public bool IsFoulTipCaught;

        /// <summary>타자 몸에 맞은(HBP) 경우.</summary>
        public bool HitBatter;
    }

    #endregion

    #region 타자 / 주자 / 득점

    /// <summary>
    /// 타자가 혹은 주자가 베이스를 부여받는(진루) 이벤트.
    /// - 볼넷, 사구, 안타, 실책 등.
    /// - RunnerManager에서 받아 주자들을 이동시킨다.
    /// </summary>
    public struct BatterAwardedBase
    {
        /// <summary>
        /// 타자에게 기본적으로 주어지는 베이스 수.
        /// - 볼넷/사구는 1, 2루타는 2, 홈런은 4 등.
        /// </summary>
        public int BaseCount;

        /// <summary>정상적인 안타로 인한 진루인지 여부.</summary>
        public bool IsHit;

        /// <summary>볼넷(4볼)에 의한 진루인지 여부.</summary>
        public bool IsWalk;

        /// <summary>사구(HBP)에 의한 진루인지 여부.</summary>
        public bool IsHitByPitch;

        /// <summary>수비 실책에 의한 진루인지 여부.</summary>
        public bool IsError;
    }

    /// <summary>
    /// 타자 아웃 타입.
    /// </summary>
    public enum BatterOutType
    {
        Unknown,
        StrikeOut,
        FlyOut,
        GroundOut,
        LineOut,
        FoulOut,
        DoublePlay,
        Other
    }

    /// <summary>
    /// 타자가 아웃 되었을 때.
    /// - 삼진, 플라이 아웃, 땅볼 포스 아웃 등.
    /// </summary>
    public struct BatterOut
    {
        public BatterOutType OutType;
    }

    /// <summary>
    /// 점수 획득 이벤트.
    /// - RunnerManager 등에서 득점 상황을 계산한 뒤 발행.
    /// </summary>
    public struct RunsScored
    {
        /// <summary>득점을 한 팀.</summary>
        public TeamSide Team;

        /// <summary>이번 이벤트에서 몇 점이 들어갔는지.</summary>
        public int RunCount;
    }

    /// <summary>
    /// 특정 주자를 n베이스 더 진루시키고 싶을 때 사용하는 요청.
    /// - 도루, 땅볼로 인한 강제 진루 등을 RunnerManager가 처리하도록 위임.
    /// - 아직 RunnerId 구조체를 만들지 않았으므로, 간단히 정수 인덱스로 표현.
    /// </summary>
    public struct RunnerAdvanceCommand
    {
        /// <summary>
        /// 어떤 주자인지를 가리키는 인덱스/슬롯 id.
        /// 구체적인 의미는 RunnerManager 구현에서 정의.
        /// </summary>
        public int RunnerIndex;

        /// <summary>
        /// 몇 베이스 더 진루를 요청하는지.
        /// 예: 1 = 한 베이스, 2 = 두 베이스.
        /// </summary>
        public int ExtraBaseCount;
    }

    #endregion

    #region 카운트 / 이닝 / 타석

    /// <summary>
    /// CountStateModule이 업데이트된 후 발행되는 카운트 변경 이벤트.
    /// - UI, 스킬, 해설, 튜토리얼 등에서 사용.
    /// </summary>
    public struct CountChanged
    {
        public int Balls;
        public int Strikes;
        public int Outs;
    }

    /// <summary>
    /// 이닝의 절반(초/말)이 시작될 때.
    /// </summary>
    public struct InningHalfStarted
    {
        /// <summary>현재 이닝 번호 (1, 2, 3 ...).</summary>
        public int InningNumber;

        /// <summary>true면 초(원정팀 공격), false면 말(홈팀 공격).</summary>
        public bool IsTop;

        /// <summary>공격팀.</summary>
        public TeamSide Offense;

        /// <summary>수비팀.</summary>
        public TeamSide Defense;
    }

    /// <summary>
    /// 이닝의 절반이 끝났을 때.
    /// </summary>
    public struct InningHalfEnded
    {
        public int InningNumber;
        public bool IsTop;
        public TeamSide Offense;
        public TeamSide Defense;
    }

    /// <summary>
    /// 타석 결과 타입.
    /// </summary>
    public enum AtBatResultType
    {
        None,
        BallInPlay, // 인플레이 (안타/땅볼/플라이 등, 세부는 HitBallResult/BatterOut 등으로 따로 표현)
        Walk,
        StrikeOut,
        HitByPitch,
        Other
    }

    /// <summary>
    /// 하나의 타석이 시작될 때.
    /// - 어느 팀 공격인지, 몇 번 타자인지 등.
    /// - 실제 PlayerDataSO는 Lineup/Team 시스템에서 따로 참조.
    /// </summary>
    public struct AtBatStartedCommand
    {
        /// <summary>공격팀.</summary>
        public TeamSide Offense;

        /// <summary>수비팀.</summary>
        public TeamSide Defense;

        /// <summary>현재 이닝 번호.</summary>
        public int InningNumber;

        /// <summary>true면 초, false면 말.</summary>
        public bool IsTop;

        /// <summary>
        /// 타순 인덱스 (0 기반).
        /// 예: 0 = 1번 타자, 8 = 9번 타자.
        /// </summary>
        public int BattingOrderIndex;
    }

    /// <summary>
    /// 타석이 종료될 때.
    /// - 어떤 타입으로 끝났는지만 간단히 요약.
    /// - 상세 득점/주루는 다른 이벤트( RunsScored, BatterAwardedBase 등 )로 처리.
    /// </summary>
    public struct AtBatEndedCommand
    {
        public AtBatResultType ResultType;
    }

    #endregion

    #region 경기 레벨 이벤트

    /// <summary>
    /// 한 경기(매치)가 시작되었을 때.
    /// BaseballGameManager에서 발행.
    /// </summary>
    public struct MatchStarted
    {
        public TeamSide HomeTeam;
        public TeamSide AwayTeam;
    }

    /// <summary>
    /// 한 경기(매치)가 종료되었을 때.
    /// </summary>
    public struct MatchEnded
    {
        public TeamSide HomeTeam;
        public TeamSide AwayTeam;
        public int HomeScore;
        public int AwayScore;
    }

    #endregion
}
