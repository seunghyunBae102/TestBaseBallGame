// Scripts/Game/Baseball/Play/BatSwingModule.cs
using UnityEngine;
using Bash.Framework.Core;
using Bash.Framework.Core;
using Bash.Framework.Node;
using Bash.Game.Baseball.Shared;

namespace Bash.Game.Baseball.Play
{
    /// <summary>
    /// 타자의 스윙 입력/상태를 관리하는 모듈.
    /// - 실제 입력/애니메이션 시스템에서 값을 받아 SwingCommand를 생성한다.
    /// - 여기서는 "서비스" 형태로 SwingCommand를 만들고, 필요하면 이벤트로 발행한다.
    /// </summary>
    public class BatSwingModule : Module
    {
        private SwingCommand _lastSwing;

        /// <summary>
        /// 스윙 정보를 등록한다.
        /// - 실제 입력 시스템에서 이 메서드를 호출하면 된다.
        /// </summary>
        public SwingCommand RegisterSwing(
            TeamSide batterTeam,
            ActorNode batterActor,
            bool didSwing,
            bool isBunt,
            Vector3 swingPlaneNormal,
            float swingTiming,
            float swingHeightRatio)
        {
            _lastSwing = new SwingCommand
            {
                batterTeam = batterTeam,
                batterActor = batterActor,
                didSwing = didSwing,
                isBunt = isBunt,
                swingPlaneNormal = swingPlaneNormal,
                swingTiming = swingTiming,
                swingHeightRatio = swingHeightRatio
            };

            // 필요하면 이벤트 버스로 발행
            Events.Publish(_lastSwing);
            return _lastSwing;
        }

        /// <summary>
        /// 마지막으로 등록된 스윙 정보를 반환한다. (해당 투구에 대한 SwingCommand)
        /// </summary>
        public SwingCommand GetLastSwing()
        {
            return _lastSwing;
        }
    }
}
