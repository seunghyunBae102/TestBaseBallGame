// PitchOrchestratorModule.cs (수정 버전)
using System.Collections.Generic;
using UnityEngine;
using Bash.Framework.Core;
using Bash.Framework.Core;
using Bash.Game.Baseball.Shared;
using Bash.Game.Baseball.Ball;
using Bash.Game.Baseball.Play; // PitchCommand struct

namespace Bash.Game.Baseball.Play
{
    public class PitchOrchestratorModule : Module
    {
        [Header("Dependencies")]
        [SerializeField] private BallFactoryModule ballFactory;
        [SerializeField] private BallConfigSetSO defaultPitchPreset;

        private BallCore _currentPitchBall;
        private bool _pitchInProgress;

        protected override void OnInit()
        {
            if (ballFactory == null)
            {
                ballFactory = GameRoot.Instance.GetManager<BallFactoryModule>();
            }

            // ★ PitchCommand 이벤트 구독
            Events.Subscribe<PitchCommand>(OnPitchCommand);
        }

        protected override void OnShutdown()
        {
            // ★ 구독 해제
            Events.Unsubscribe<PitchCommand>(OnPitchCommand);
        }

        private void OnPitchCommand(PitchCommand cmd)
        {
            StartPitch(in cmd);
        }

        public BallCore StartPitch(in PitchCommand cmd)
        {
            if (_pitchInProgress && _currentPitchBall != null)
            {
                Debug.LogWarning("[PitchOrchestrator] 이전 투구가 아직 끝나지 않았습니다. 강제로 새 투구를 시작합니다.");
            }

            var preset = cmd.ballPreset != null ? cmd.ballPreset : defaultPitchPreset;
            if (preset == null || preset.behaviour == null)
            {
                Debug.LogError("[PitchOrchestrator] BallConfigSetSO 혹은 Behaviour가 설정되어 있지 않습니다.");
                return null;
            }

            Vector3 dir = (cmd.targetPoint - cmd.releasePosition).normalized;
            if (dir.sqrMagnitude < 0.0001f)
                dir = Vector3.forward;

            float power = Mathf.Max(0.1f, cmd.powerFactor);
            float speed = preset.baseSpeed * power;

            BallSpawnContext context = new BallSpawnContext
            {
                spawnPosition = cmd.releasePosition,
                initialVelocity = dir * speed,
                spinAxis = Vector3.forward,
                spinRate = preset.baseSpinRate * power,
                initialPhase = BallPhase.Pitched,
                ownerActor = cmd.pitcherActor,
                ownerTeam = cmd.pitcherTeam,
                powerFactor = power,
                contactQuality = 0f
            };

            IList<BallSkillSO> skills = preset.skills != null ? (IList<BallSkillSO>)preset.skills : null;

            _currentPitchBall = ballFactory.SpawnBall(in context, preset.behaviour, skills);
            _pitchInProgress = _currentPitchBall != null;

            return _currentPitchBall;
        }

        public void EndCurrentPitch(bool despawnBall = false)
        {
            if (!_pitchInProgress || _currentPitchBall == null)
            {
                _pitchInProgress = false;
                _currentPitchBall = null;
                return;
            }

            if (despawnBall)
            {
                _currentPitchBall.Despawn();
            }

            _pitchInProgress = false;
            _currentPitchBall = null;
        }

        public BallCore GetCurrentPitchBall()
        {
            return _currentPitchBall;
        }
    }
}
