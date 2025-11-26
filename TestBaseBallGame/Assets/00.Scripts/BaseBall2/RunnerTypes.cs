//// Scripts/Game/Baseball/Shared/RunnerTypes.cs
//using Bash.Framework.Node;

//namespace Bash.Game.Baseball.Shared
//{
//    /// <summary>야구 베이스 인덱스 (Home=타석, 1루, 2루, 3루).</summary>
//    public enum BaseIndex
//    {
//        Home = 0,
//        First = 1,
//        Second = 2,
//        Third = 3
//    }

//    /// <summary>한 명의 주자를 식별하는 최소 단위.</summary>
//    public struct RunnerId
//    {
//        public int playerIndex; // 타순 인덱스 등
//        public TeamSide team;
//        public ActorNode actor;      // 실제 ActorNode (없으면 null 허용)

//        public RunnerId(int playerIndex, TeamSide team, ActorNode actor = null)
//        {
//            this.playerIndex = playerIndex;
//            this.team = team;
//            this.actor = actor;
//        }

//        public override string ToString()
//        {
//            return $"Runner[{team}, idx={playerIndex}]";
//        }
//    }

//    public static class RunnerUtil
//    {
//        /// <summary>RunnerId가 같은 사람인지 비교할 때 사용 (actor는 무시).</summary>
//        public static bool Equals(in RunnerId a, in RunnerId b)
//        {
//            return a.playerIndex == b.playerIndex && a.team == b.team;
//        }
//    }
//}
