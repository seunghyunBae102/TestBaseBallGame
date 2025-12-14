using Bash.Core.Ball;
using Bash.Core.GamePlay.Environment;
using Bash.Core.Unit;
using Bash.Framework.Core;
using Bash.Framework.Core.Events;
using Bash.Framework.Utils; // Ballistics
using UnityEngine;

namespace Bash.Core.GamePlay
{
    public class FielderPawn : UnitPawn
    {
        [Header("Fielder Sockets")]
        [SerializeField] private Transform _gloveHand;
        [SerializeField] private Transform _throwHand;

        private BallNode _heldBall;

        public void CatchBall(BallNode ball)
        {
            if (ball == null || _gloveHand == null) return;


            _heldBall = ball;

            var phys = ball.GetCompo<BallPhysicsCompo>();
            if (phys != null) phys.StopSimulation(); // [Fix] 메서드 호출 가능

            ball.AttachTo(this);
            ball.transform.SetParent(_gloveHand);
            ball.transform.localPosition = Vector3.zero;
            ball.transform.localRotation = Quaternion.identity;

            Movement.Stop();
            GameRoot.Instance.Events.Publish(new BallCaughtEvent { Catcher = this });
        }

        public void ThrowBall(BaseNode targetBase)
        {
            if (_heldBall == null) return;
            if (targetBase == null) return;

            Vector3 origin = _throwHand.position;
            Vector3 targetPos = targetBase.GetCatchPosition();
            float dist = Vector3.Distance(origin, targetPos);

            // 스탯 적용
            float dex = Stat ? Stat.GetStat(EStatType.Dexterity) : 50f;
            Vector3 finalTarget = Ballistics.ApplyErrorToTarget(targetPos, dist, dex);

            float str = Stat ? Stat.GetStat(EStatType.Strength) : 50f;
            float thr = Stat ? Stat.GetStat(EStatType.Throwing) : 50f;
            float powerStat = (thr * 0.7f) + (str * 0.3f);

            float flightTime = Ballistics.CalculateFlightTime(origin, finalTarget, powerStat);
            float gravity = 15.0f;
            Vector3 velocity = Ballistics.CalculateVelocity(origin, finalTarget, flightTime, gravity);

            ReleaseBall(velocity, gravity);
        }

        private void ReleaseBall(Vector3 velocity, float gravity)
        {
            _heldBall.AttachTo(GameRoot.Instance.WorldRoot);
            _heldBall.transform.position = _throwHand.position;
            _heldBall.transform.rotation = Quaternion.LookRotation(velocity);

            var strategy = new HitPhysicsStrategy(gravity: gravity, drag: 0.05f);
            var phys = _heldBall.GetCompo<BallPhysicsCompo>();

            // [Fix] 메서드 호출 가능
            phys.Launch(velocity.normalized, velocity.magnitude, strategy);

            _heldBall = null;
        }
    }
}