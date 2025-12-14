using UnityEngine;
using Bash.Framework.Core;
using Bash.Framework.Managers;
using Bash.Core.GamePlay.Environment;

namespace Bash.Core.Rules
{
    public enum EOutType
    {
        None,       // 아웃 아님 (세이프 등)
        ForceOut,   // 베이스만 밟으면 됨 (공이 사람보다 빠름)
        TagOut      // 사람을 맞춰야 함 (태그 필요)
    }

    public class OutLogicManager : ActorCompo
    {
        private FieldManager _fieldMgr;

        protected override void OnInit()
        {
            _fieldMgr = GameRoot.Instance.GetManager<FieldManager>();
        }

        /// <summary>
        /// 특정 베이스로 공을 던졌을 때, 어떤 아웃 판정이 적용되는지 계산
        /// </summary>
        /// <param name="targetBase">목표 베이스</param>
        /// <param name="isGroundBall">땅볼인가? (뜬공이면 로직이 달라짐)</param>
        public EOutType GetOutType(BaseNode.EBaseType targetBase, bool isGroundBall)
        {
            if (_fieldMgr == null || _fieldMgr.CurrentField == null) return EOutType.None;
            var field = _fieldMgr.CurrentField;

            // 1. 뜬공(Fly Ball)일 경우:
            // 타자 주자는 아웃되었거나(잡힘), 안타임. 
            // 진루 의무가 없으므로 모든 진루 시도는 '태그 아웃' 대상.
            // (단, 귀루 후 태그업 상황은 별도 로직이나 여기선 일단 태그아웃으로 통일)
            if (!isGroundBall)
            {
                return EOutType.TagOut;
            }

            // 2. 땅볼(Ground Ball)일 경우 포스 플레이 체크:
            // 해당 베이스로 가야만 하는 '의무(Force)'가 타자로부터 끊기지 않고 이어지는가?

            switch (targetBase)
            {
                case BaseNode.EBaseType.First:
                    // 타자 주자는 땅볼 시 무조건 1루로 가야 함 -> 항상 포스 아웃
                    return EOutType.ForceOut;

                case BaseNode.EBaseType.Second:
                    // 1루에 주자가 있어야만 2루가 포스 상태가 됨
                    return IsOccupied(BaseNode.EBaseType.First) ? EOutType.ForceOut : EOutType.TagOut;

                case BaseNode.EBaseType.Third:
                    // 1루 AND 2루에 주자가 있어야 3루가 포스 상태
                    return (IsOccupied(BaseNode.EBaseType.First) && IsOccupied(BaseNode.EBaseType.Second))
                           ? EOutType.ForceOut : EOutType.TagOut;

                case BaseNode.EBaseType.Home:
                    // 만루(1,2,3루 모두 점유)여야 홈이 포스 상태
                    return (IsOccupied(BaseNode.EBaseType.First) &&
                            IsOccupied(BaseNode.EBaseType.Second) &&
                            IsOccupied(BaseNode.EBaseType.Third))
                           ? EOutType.ForceOut : EOutType.TagOut;
            }

            return EOutType.None;
        }

        // 헬퍼: 해당 베이스에 주자가 있는가?
        private bool IsOccupied(BaseNode.EBaseType type)
        {
            var node = _fieldMgr.CurrentField.GetBase(type);
            return node != null && node.IsOccupied;
        }
    }
}