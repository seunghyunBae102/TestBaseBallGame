using System.Collections.Generic;
using UnityEngine;
using Bash.Framework.Core;

namespace Bash.Core.GamePlay.Environment
{
    public class PlayField : ActorNode
    {
        [Header("Links (Order: Home, 1B, 2B, 3B)")]
        [SerializeField] private BaseNode[] _bases;

        private Dictionary<BaseNode.EBaseType, BaseNode> _baseMap;

        protected override void Awake()
        {
            base.Awake();
            _baseMap = new Dictionary<BaseNode.EBaseType, BaseNode>();
            if (_bases != null)
            {
                foreach (var b in _bases)
                {
                    if (b != null) _baseMap[b.BaseType] = b;
                }
            }
        }

        public BaseNode GetBase(BaseNode.EBaseType type) => _baseMap.TryGetValue(type, out var n) ? n : null;

        public BaseNode GetNextBase(BaseNode.EBaseType current)
        {
            int nextIdx = ((int)current + 1) % 4;
            return GetBase((BaseNode.EBaseType)nextIdx);
        }
    }
}