// Scripts/Game/Baseball/Events/RunnerEvents.cs
using Bash.Framework.Node;
using Bash.Game.Baseball.Shared;

namespace Bash.Game.Baseball.Events
{
    /// <summary>베이스에 서 있는 주자를 식별하기 위한 ID.</summary>
    public struct RunnerId
    {
        public int batterIndex;  // 타순 인덱스 등
        public TeamSide team;    // 공격 팀
        public ActorNode actor;  // 실제 캐릭터 (없으면 null 허용)

        public RunnerId(int batterIndex, TeamSide team, ActorNode actor = null)
        {
            this.batterIndex = batterIndex;
            this.team = team;
            this.actor = actor;
        }

        public override string ToString()
        {
            return $"RunnerId(batter={batterIndex}, team={team})";
        }
    }

    /// <summary>베이스 인덱스: 0=홈, 1=1루, 2=2루, 3=3루.</summary>
    public enum BaseIndex
    {
        Home = 0,
        First = 1,
        Second = 2,
        Third = 3
    }

    /// <summary>주루 요청: 특정 러너를 몇 루만큼 이동시키라는 요청.</summary>
    public struct RunnerAdvanceRequest
    {
        public RunnerId Runner;
        public BaseIndex FromBase;
        public int AdvanceCount; // +1 (한 루), +2, +3, +4(홈까지)
        public bool Forced;      // 포스 플레이인지 여부
    }

    /// <summary>주자가 아웃되었을 때.</summary>
    public enum OutReason
    {
        StrikeOut,
        FlyOut,
        TagOut,
        ForceOut,
        Other
    }

    public struct RunnerOut
    {
        public RunnerId Runner;
        public BaseIndex AtBase;
        public OutReason Reason;
    }

    /// <summary>러너가 득점했을 때.</summary>
    public struct RunnerScored
    {
        public RunnerId Runner;
        public int Runs; // 보통 1
    }

    /// <summary>타석 종료 이유.</summary>
    public enum AtBatEndReason
    {
        StrikeOut,
        Walk,
        HitByPitch,
        HitInPlay,
        FlyOut,
        SacFly,
        Other
    }

    /// <summary>타석 시작 이벤트 (PlayFlow에서 발행).</summary>
    public struct AtBatStarted
    {
        public TeamSide OffenseTeam;
        public RunnerId Batter;
        public int BattingOrderIndex;
    }

    /// <summary>타석 종료 이벤트 (CountState/RunnerRule/PlayFlow에서 발행).</summary>
    public struct AtBatEnded
    {
        public TeamSide OffenseTeam;
        public RunnerId Batter;
        public AtBatEndReason Reason;
    }
}
