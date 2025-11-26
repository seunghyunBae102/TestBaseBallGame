// Scripts/Game/Baseball/Fielder/FielderStateModule.cs
using Bash.Framework.Core;
using Bash.Framework.Node;
using System.Collections.Generic;
using UnityEngine;

namespace Bash.Game.Baseball.Fielder
{
    /// <summary>수비 포지션 종류.</summary>
    public enum FieldPosition
    {
        Pitcher,
        Catcher,
        FirstBase,
        SecondBase,
        ThirdBase,
        ShortStop,
        LeftField,
        CenterField,
        RightField
    }

    /// <summary>포지션 하나에 대한 슬롯.</summary>
    [System.Serializable]
    public struct FielderSlot
    {
        public FieldPosition position;
        public ActorNode actor;
    }

    /// <summary>
    /// 어느 포지션에 어떤 수비수가 서 있는지 상태만 관리하는 모듈.
    /// - AI/입력 시스템은 이 정보를 보고 행동하면 된다.
    /// </summary>
    public class FielderStateModule : Module
    {
        [Header("Initial Defense")]
        [Tooltip("매치 시작 시 기본 수비 포지션 세팅.")]
        [SerializeField] private FielderSlot[] initialSlots;

        private readonly Dictionary<FieldPosition, ActorNode> _fielders =
            new Dictionary<FieldPosition, ActorNode>();

        protected override void OnInit()
        {
            _fielders.Clear();

            // 초기 세팅
            if (initialSlots != null)
            {
                foreach (var slot in initialSlots)
                {
                    if (!_fielders.ContainsKey(slot.position))
                        _fielders.Add(slot.position, slot.actor);
                    else
                        _fielders[slot.position] = slot.actor;
                }
            }

            // 필요하다면 MatchStarted/InningHalfStarted 등을 구독하여
            // 이닝 전환 시 수비 교체/시프트 등을 적용할 수 있다.
        }

        /// <summary>특정 포지션에 수비수를 설정.</summary>
        public void SetFielder(FieldPosition pos, ActorNode actor)
        {
            _fielders[pos] = actor;
        }

        /// <summary>특정 포지션의 수비수를 얻는다 (없으면 null).</summary>
        public ActorNode GetFielder(FieldPosition pos)
        {
            _fielders.TryGetValue(pos, out var actor);
            return actor;
        }

        /// <summary>현재 모든 포지션-수비수 매핑을 배열로 반환.</summary>
        public FielderSlot[] GetAllSlots()
        {
            var arr = new FielderSlot[_fielders.Count];
            int i = 0;
            foreach (var kv in _fielders)
            {
                arr[i] = new FielderSlot
                {
                    position = kv.Key,
                    actor = kv.Value
                };
                i++;
            }
            return arr;
        }
    }
}
