// HitResultRuleModule.cs (수정 버전)
using UnityEngine;
using Bash.Framework.Core;

using Bash.Game.Baseball.Events;


namespace Bash.Game.Baseball.Play
{
    public class HitResultRuleModule : Module
    {
        protected override void OnInit()
        {
            // ★ HitResultRequest 이벤트 구독
            Events.Subscribe<HitResultRequest>(OnHitResultRequest);
        }

        protected override void OnShutdown()
        {
            Events.Unsubscribe<HitResultRequest>(OnHitResultRequest);
        }

        private void OnHitResultRequest(HitResultRequest req)
        {
            EvaluateAndPublish(
                req.IsFair,
                req.IsHomeRun,
                req.BallCaughtInAir,
                req.LandedInFoulTerritory,
                req.Distance);
        }

        public HitBallResult Evaluate(
            bool isFair,
            bool isHomeRun,
            bool ballCaughtInAir,
            bool landedInFoulTerritory,
            float distance)
        {
            HitBallResult result = new HitBallResult();

            if (!isFair || landedInFoulTerritory)
            {
                result.IsFair = false;
                result.HitType = HitType.FoulBall;
                result.IsHomeRun = false;
                result.IsCaughtInAir = false;
                result.BaseCount = 0;
                return result;
            }

            result.IsFair = true;

            if (ballCaughtInAir)
            {
                result.HitType = HitType.FlyBall;
                result.IsCaughtInAir = true;
                result.BaseCount = 0;
                result.IsHomeRun = false;
                return result;
            }

            if (isHomeRun)
            {
                result.HitType = HitType.HomeRun;
                result.IsHomeRun = true;
                result.IsCaughtInAir = false;
                result.BaseCount = 4;
                return result;
            }

            result.IsCaughtInAir = false;
            result.IsHomeRun = false;

            if (distance < 60f)
            {
                result.HitType = HitType.GroundBall;
                result.BaseCount = 1;
            }
            else if (distance < 90f)
            {
                result.HitType = HitType.LineDrive;
                result.BaseCount = 2;
            }
            else
            {
                result.HitType = HitType.LineDrive;
                result.BaseCount = 3;
            }

            return result;
        }

        public void EvaluateAndPublish(
            bool isFair,
            bool isHomeRun,
            bool ballCaughtInAir,
            bool landedInFoulTerritory,
            float distance)
        {
            var result = Evaluate(isFair, isHomeRun, ballCaughtInAir, landedInFoulTerritory, distance);
            Events.Publish(result);
        }
    }
}
