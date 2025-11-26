// Scripts/Game/Baseball/Ball/BallFactoryModule.cs
using Bash.Framework.Core;
using Bash.Framework.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Bash.Game.Baseball.Ball
{
    /// <summary>
    /// 공을 생성/회수하는 팩토리 모듈.
    /// - Pitch/Hit/Throw 모듈에서 직접 Instantiate하지 않고,
    ///   이 모듈을 통해 공을 만들도록 통일하면 나중에 풀링으로 바꾸기 쉽다.
    /// </summary>
    public class BallFactoryModule : Module
    {
        [Header("Prefab / Parent")]
        [SerializeField] private BallCore ballPrefab;
        [SerializeField] private Transform defaultParent;

        // 나중에 풀링을 붙이고 싶으면 여기에서 PoolManager를 참조하면 된다.

        /// <summary>
        /// 공 하나를 스폰하고, BallCore.Setup을 호출한 뒤 반환한다.
        /// </summary>
        public BallCore SpawnBall(
            in BallSpawnContext context,
            BallBehaviourSO behaviour,
            IList<BallSkillSO> skills)
        {
            if (ballPrefab == null)
            {
                Debug.LogError("[BallFactoryModule] ballPrefab이 설정되어 있지 않습니다.");
                return null;
            }

            Transform parent = defaultParent != null ? defaultParent : null;

            BallCore instance = Instantiate(ballPrefab, context.spawnPosition, Quaternion.identity, parent);
            instance.name = $"{ballPrefab.name}_Runtime";

            instance.Setup(in context, behaviour, skills);
            return instance;
        }
    }
}
