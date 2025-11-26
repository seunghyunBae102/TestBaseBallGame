// HitOrchestratorModule.cs (수정 버전)
using Bash.Framework.Core;
using Bash.Framework.Core;
using Bash.Game.Baseball.Ball;
using Bash.Game.Baseball.Events;
using System.Collections.Generic;
using UnityEngine;

namespace Bash.Game.Baseball.Play
{
    public class HitOrchestratorModule : Module
    {
        [Header("Dependencies")]
        [SerializeField] private BallFactoryModule ballFactory;

        [Header("Default Hit Behaviours")]
        [SerializeField] private BallBehaviourSO flyOrLineDriveBehaviour;
        [SerializeField] private BallBehaviourSO groundBallBehaviour;
        [SerializeField] private BallSkillSO[] defaultHitSkills;

        protected override void OnInit()
        {
            if (ballFactory == null)
                ballFactory = GameRoot.Instance.GetManager<BallFactoryModule>();

            // ★ HitBallSpawnContext 이벤트 구독
            Events.Subscribe<HitBallSpawnContext>(OnHitBallSpawnContext);
        }

        protected override void OnShutdown()
        {
            Events.Unsubscribe<HitBallSpawnContext>(OnHitBallSpawnContext);
        }

        private void OnHitBallSpawnContext(HitBallSpawnContext ctx)
        {
            SpawnHitBall(in ctx);
        }

        public BallCore SpawnHitBall(in HitBallSpawnContext hitCtx)
        {
            if (ballFactory == null)
            {
                Debug.LogError("[HitOrchestrator] BallFactoryModule이 설정되어 있지 않습니다.");
                return null;
            }

            BallBehaviourSO behaviourToUse = null;
            switch (hitCtx.predictedHitType)
            {
                case HitType.GroundBall:
                    behaviourToUse = groundBallBehaviour != null ? groundBallBehaviour : flyOrLineDriveBehaviour;
                    break;
                case HitType.LineDrive:
                case HitType.FlyBall:
                case HitType.HomeRun:
                default:
                    behaviourToUse = flyOrLineDriveBehaviour;
                    break;
            }

            if (behaviourToUse == null)
            {
                Debug.LogError("[HitOrchestrator] 사용할 타구 Behaviour가 설정되어 있지 않습니다.");
                return null;
            }

            BallSpawnContext baseCtx = hitCtx.baseContext;

            IList<BallSkillSO> skills = defaultHitSkills != null
                ? (IList<BallSkillSO>)defaultHitSkills
                : null;

            BallCore hitBall = ballFactory.SpawnBall(in baseCtx, behaviourToUse, skills);
            return hitBall;
        }
    }
}
