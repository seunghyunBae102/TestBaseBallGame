using UnityEngine;
using Bash.Framework.Core;
using Bash.Core.GamePlay.Environment;

namespace Bash.Framework.Managers
{
    public class FieldManager : ActorCompo
    {
        private PlayField _cachedField;

        public PlayField CurrentField
        {
            get
            {
                if (_cachedField != null) return _cachedField;

                // 월드에서 검색 (Lazy Loading)
                _cachedField = GameRoot.Instance.FindNode<PlayField>();

                if (_cachedField == null)
                    Debug.LogWarning("[FieldManager] PlayField not found in scene!");

                return _cachedField;
            }
        }

        public void ClearCache() => _cachedField = null;
    }
}