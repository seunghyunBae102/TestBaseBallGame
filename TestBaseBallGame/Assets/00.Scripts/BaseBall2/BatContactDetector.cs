// BatContactDetector.cs (수정 버전)
using Bash.Framework.Core;
using Bash.Framework.Node;
using Bash.Game.Baseball.Ball;
using Bash.Game.Baseball.Config;
using Bash.Game.Baseball.Events;
using UnityEngine;

namespace Bash.Game.Baseball.Play
{
    public class BatContactDetector : BashNode
    {
        [Header("References")]
        [SerializeField] private PlayerDataSO batterData;

        [Header("Dependencies")]
        [SerializeField] private BatSwingModule swingModule;

        protected override void Awake()
        {
            base.Awake();

            if (swingModule == null)
            {
                // 씬 어딘가의 BatSwingModule을 찾아오는 간단한 방식
                swingModule = GameRoot.Instance.GetManager<BatSwingModule>();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            TryHandleContact(collision.collider, collision.GetContact(0).point);
        }

        private void OnTriggerEnter(Collider other)
        {
            // 필요하다면 트리거 기반으로도 처리
            TryHandleContact(other, transform.position);
        }

        private void TryHandleContact(Collider other, Vector3 hitPoint)
        {
            var ball = other.GetComponentInParent<BallCore>();
            if (ball == null)
                return;

            if (swingModule == null)
            {
                Debug.LogWarning("[BatContactDetector] swingModule이 설정되지 않았습니다.");
                return;
            }

            var swingCommand = swingModule.GetLastSwing();

            // 배트 속도는 일단 단순화: 배트의 현재 속도(없으면 forward 방향)
            Vector3 batVelocity = Vector3.zero;
            var rb = GetComponentInParent<Rigidbody>();
            if (rb != null)
                batVelocity = rb.linearVelocity;
            if (batVelocity.sqrMagnitude < 0.001f)
                batVelocity = transform.forward * 10f;

            HitBallSpawnContext hitCtx = ComputeHitContext(ball, hitPoint, batVelocity, in swingCommand);

            // ★ HitBallSpawnContext 이벤트로 발행
            GameRoot.Instance.Events.Publish(hitCtx);
        }

        public HitBallSpawnContext ComputeHitContext(
            BallCore ball,
            Vector3 hitPoint,
            Vector3 batVelocity,
            in SwingCommand swingCommand)
        {
            HitBallSpawnContext ctx = new HitBallSpawnContext();

            if (ball == null)
                return ctx;

            Vector3 dir = batVelocity.normalized;
            if (dir.sqrMagnitude < 0.0001f)
                dir = Vector3.forward;

            float powerStat = 50f;
            float contactStat = 50f;

            if (batterData != null)
            {
                powerStat = batterData.stats.batting.power;
                contactStat = batterData.stats.batting.contact;
            }

            float powerFactor = Mathf.Lerp(0.6f, 1.4f, powerStat / 100f);
            float contactFactor = Mathf.Lerp(0.5f, 1.2f, contactStat / 100f);

            float timingQuality = 1f - Mathf.Abs(swingCommand.swingTiming - 0.5f) * 2f;
            timingQuality = Mathf.Clamp01(timingQuality);

            float exitVelocity = 30f * powerFactor * (0.7f + 0.3f * timingQuality);
            float contactQuality = contactFactor * timingQuality;

            float launchAngle = Mathf.Lerp(5f, 45f, swingCommand.swingHeightRatio);
            float sprayAngle = 0f;

            ctx.baseContext = new BallSpawnContext
            {
                spawnPosition = hitPoint,
                initialVelocity = dir * exitVelocity,
                spinAxis = Vector3.up,
                spinRate = 500f,
                initialPhase = BallPhase.Hit,
                ownerActor = swingCommand.batterActor,
                ownerTeam = swingCommand.batterTeam,
                powerFactor = powerFactor,
                contactQuality = contactQuality
            };

            ctx.exitVelocity = exitVelocity;
            ctx.launchDirection = dir;
            ctx.launchAngle = launchAngle;
            ctx.sprayAngle = sprayAngle;
            ctx.contactQuality = contactQuality;

            if (launchAngle > 30f)
                ctx.predictedHitType = HitType.FlyBall;
            else if (launchAngle > 10f)
                ctx.predictedHitType = HitType.LineDrive;
            else
                ctx.predictedHitType = HitType.GroundBall;

            return ctx;
        }
    }
}
