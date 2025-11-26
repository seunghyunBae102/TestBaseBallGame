// PitchOutcomeRuleModule.cs (수정 버전)
using UnityEngine;
using Bash.Framework.Core;
using Bash.Framework.Core;
using Bash.Game.Baseball.Events;

namespace Bash.Game.Baseball.Play
{
    public class PitchOutcomeRuleModule : Module
    {
        protected override void OnInit()
        {
            // ★ PitchOutcomeRequest 이벤트 구독
            Events.Subscribe<PitchOutcomeRequest>(OnPitchOutcomeRequest);
        }

        protected override void OnShutdown()
        {
            Events.Unsubscribe<PitchOutcomeRequest>(OnPitchOutcomeRequest);
        }

        private void OnPitchOutcomeRequest(PitchOutcomeRequest req)
        {
            EvaluateAndPublish(
                req.BatterSwung,
                req.MadeContact,
                req.IsInStrikeZone,
                req.IsFoul,
                req.HitBatter);
        }

        public PitchOutcome Evaluate(
            bool batterSwung,
            bool madeContact,
            bool isInStrikeZone,
            bool isFoul,
            bool hitBatter)
        {
            PitchOutcome outcome = new PitchOutcome();

            if (hitBatter)
            {
                outcome.Type = PitchOutcomeType.HitByPitch;
                return outcome;
            }

            if (!madeContact && !batterSwung)
            {
                outcome.Type = isInStrikeZone ? PitchOutcomeType.CalledStrike : PitchOutcomeType.Ball;
                return outcome;
            }

            if (!madeContact && batterSwung)
            {
                outcome.Type = PitchOutcomeType.SwingingStrike;
                return outcome;
            }

            if (isFoul)
            {
                outcome.Type = PitchOutcomeType.Foul;
                return outcome;
            }

            outcome.Type = PitchOutcomeType.InPlayFair;
            return outcome;
        }

        public void EvaluateAndPublish(
            bool batterSwung,
            bool madeContact,
            bool isInStrikeZone,
            bool isFoul,
            bool hitBatter)
        {
            var outcome = Evaluate(batterSwung, madeContact, isInStrikeZone, isFoul, hitBatter);
            Events.Publish(outcome);
        }
    }
}
