using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Bash.Framework.Core;

namespace Bash.Framework.Managers
{
    public class ResourceManager : ActorCompo
    {
        // 로드된 핸들 캐싱 (메모리 해제용)
        private Dictionary<string, AsyncOperationHandle<GameObject>> _loadedHandles
            = new Dictionary<string, AsyncOperationHandle<GameObject>>();

        public async Task<GameObject> LoadModelAsync(string addressID)
        {
            if (string.IsNullOrEmpty(addressID)) return null;

            // 캐시 확인
            if (_loadedHandles.ContainsKey(addressID))
            {
                var handle = _loadedHandles[addressID];
                if (handle.Status == AsyncOperationStatus.Succeeded) return handle.Result;
            }

            // 비동기 로드
            var opHandle = Addressables.LoadAssetAsync<GameObject>(addressID);
            await opHandle.Task;

            if (opHandle.Status == AsyncOperationStatus.Succeeded)
            {
                if (!_loadedHandles.ContainsKey(addressID)) _loadedHandles.Add(addressID, opHandle);
                return opHandle.Result;
            }

            Debug.LogError($"[Resource] Failed load: {addressID}");
            return null;
        }

        public void ReleaseAll()
        {
            foreach (var handle in _loadedHandles.Values) Addressables.Release(handle);
            _loadedHandles.Clear();
        }

        protected override void OnDestroy() { base.OnDestroy(); ReleaseAll(); }
    }
}