// Scripts/Game/Baseball/Sensing/PitchZoneSensor.cs
using UnityEngine;
using Bash.Framework.Node;
using Bash.Framework.Core;
using Bash.Game.Baseball.Ball;
using Bash.Game.Baseball.Events;
using Bash.Game.Baseball.Play;

namespace Bash.Game.Baseball.Sensing
{
    /// <summary>
    /// 스트라이크 존 / 타자 / 포수 / 백넷을 감지해서
    /// 한 번의 투구가 끝났을 때 PitchOutcomeRequest를 발행하는 센서.
    /// </summary>
    public class PitchZoneSensor : BashNode
    {
        [Header("Zone References")]
        [Tooltip("스트라이크 존 전체를 감싸는 Trigger Collider.")]
        [SerializeField] private Collider strikeZoneVolume;

        [Tooltip("타자 몸통/헬멧 등을 감싸는 Collider (Hit by pitch 판정용).")]
        [SerializeField] private Collider batterHitbox;

        [Tooltip("포수 글러브 영역 (공 잡는 위치)")]
        [SerializeField] private Collider catcherGlove;

        [Tooltip("백넷/뒤쪽 벽 Trigger Collier (포수 뒤로 빠질 때용, 옵션).")]
        [SerializeField] private Collider backstopVolume;

        [Header("Timing")]
        [Tooltip("이 시간이 지나면 강제로 투구 종료로 본다.")]
        [SerializeField] private float maxPitchDuration = 5f;

        private BallCore _currentBall;
        private bool _pitchActive;
        private bool _wasInStrikeZone;
        private bool _hitBatter;
        private bool _madeContact;
        private bool _requestSent;
        private float _pitchElapsed;

        private SwingCommand _lastSwing;

        // --- EventHub 구독 ---

        protected void OnEnable()
        {
            var events = GameRoot.Instance.Events;
            events.Subscribe<SwingCommand>(OnSwingCommand);
            events.Subscribe<HitBallSpawnContext>(OnHitBallSpawnContext);
        }

        protected void OnDisable()
        {
            var events = GameRoot.Instance.Events;
            events.Unsubscribe<SwingCommand>(OnSwingCommand);
            events.Unsubscribe<HitBallSpawnContext>(OnHitBallSpawnContext);
        }

        /// <summary>
        /// 외부(예: PitchOrchestratorModule)에서 현재 투구 BallCore를 명시적으로 넘겨줄 때 사용.
        /// 꼭 안 써도 되지만 있으면 더 안전하게 추적 가능.
        /// </summary>
        public void BeginTrack(BallCore ball)
        {
            _currentBall = ball;
            _pitchActive = ball != null;
            _wasInStrikeZone = false;
            _hitBatter = false;
            _madeContact = false;
            _requestSent = false;
            _pitchElapsed = 0f;
        }

        private void OnSwingCommand(SwingCommand cmd)
        {
            _lastSwing = cmd;
        }

        /// <summary>
        /// BatContactDetector에서 HitBallSpawnContext가 발행되면,
        /// "배트와 공이 맞았다"라고 간주.
        /// </summary>
        private void OnHitBallSpawnContext(HitBallSpawnContext ctx)
        {
            _madeContact = true;
        }

        private void FixedUpdate()
        {
            if (!_pitchActive || _currentBall == null)
                return;

            _pitchElapsed += Time.fixedDeltaTime;

            // 공 Phase가 Pitched가 아니면 (Hit/Dead 등) 투구 종료로 판단
            if (_currentBall.State.Phase != BallPhase.Pitched || _pitchElapsed > maxPitchDuration)
            {
                EndPitchAndSendRequest();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var ball = other.GetComponentInParent<BallCore>();
            if (ball == null)
                return;

            // 아직 추적 중인 공이 없다면, 이 공을 현재 투구로 간주
            if (!_pitchActive)
                BeginTrack(ball);

            if (_currentBall != null && ball != _currentBall)
                return; // 동시에 여러 공을 허용하지 않는다고 가정

            if (other == batterHitbox)
            {
                _hitBatter = true;
                // 타자 몸에 맞은 순간 투구 종료로 처리
                EndPitchAndSendRequest();
            }
            else if (other == strikeZoneVolume)
            {
                _wasInStrikeZone = true;
            }
            else if (other == catcherGlove || other == backstopVolume)
            {
                // 포수가 잡았거나 뒤로 빠졌다면 투구 종료
                EndPitchAndSendRequest();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!_pitchActive || _currentBall == null)
                return;

            if (other == strikeZoneVolume)
            {
                var ball = other.GetComponentInParent<BallCore>();
                if (ball != null && ball == _currentBall)
                    _wasInStrikeZone = true;
            }
        }

        /// <summary>
        /// 투구 종료 시 PitchOutcomeRequest 이벤트를 날리고 내부 상태를 리셋.
        /// </summary>
        private void EndPitchAndSendRequest()
        {
            if (!_pitchActive || _requestSent)
                return;

            _pitchActive = false;
            _requestSent = true;

            var req = new PitchOutcomeRequest
            {
                BatterSwung = _lastSwing.didSwing,
                MadeContact = _madeContact,
                IsInStrikeZone = _wasInStrikeZone,
                // TODO: 파울 전용 센서/판정이 생기면 여기 채우면 됨.
                IsFoul = false,
                HitBatter = _hitBatter
            };

            GameRoot.Instance.Events.Publish(req);

            _currentBall = null;
        }
    }
}
