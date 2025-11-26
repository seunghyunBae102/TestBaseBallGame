// Scripts/Game/Baseball/Sensing/FieldHitSensor.cs
using UnityEngine;
using Bash.Framework.Node;
using Bash.Framework.Core;
using Bash.Game.Baseball.Ball;
using Bash.Game.Baseball.Events;
using Bash.Game.Baseball.Play;

namespace Bash.Game.Baseball.Sensing
{
    /// <summary>
    /// 타구(Hit Phase 공)가 필드와 상호작용하는 것을 감지해서
    /// HitResultRequest를 발행하는 센서.
    /// </summary>
    public class FieldHitSensor : BashNode
    {
        [Header("Volumes / Colliders")]
        [Tooltip("페어 구역 전체를 감싸는 Collider (대략적인 영역이면 충분).")]
        [SerializeField] private Collider fairTerritoryVolume;

        [Tooltip("좌측 파울 구역 Collider (옵션).")]
        [SerializeField] private Collider foulTerritoryVolumeLeft;

        [Tooltip("우측 파울 구역 Collider (옵션).")]
        [SerializeField] private Collider foulTerritoryVolumeRight;

        [Tooltip("홈런으로 인정되는 뒤쪽/펜스 뒤 볼륨.")]
        [SerializeField] private Collider homeRunVolume;

        [Tooltip("땅(필드) Collider. 여기와 충돌하면 타구가 끝났다고 본다.")]
        [SerializeField] private Collider groundCollider;

        [Header("Catch")]
        [Tooltip("수비수 글러브에 붙는 Tag 이름.")]
        [SerializeField] private string fielderGloveTag = "FielderGlove";

        [Header("Timing")]
        [Tooltip("이 시간이 지나도록 타구가 안 끝나면 강제로 종료 처리.")]
        [SerializeField] private float maxHitDuration = 8f;

        private bool _trackingHit;
        private bool _hasLaunchPos;
        private Vector3 _launchPos;

        private bool _isFair;
        private bool _landedInFoul;
        private bool _isHomeRun;
        private bool _caughtInAir;
        private bool _requestSent;

        private float _elapsed;

        // --- HitBallSpawnContext 수신 (타구 시작 시점) ---

        protected void OnEnable()
        {
            GameRoot.Instance.Events.Subscribe<HitBallSpawnContext>(OnHitBallSpawnContext);
        }

        protected void OnDisable()
        {
            GameRoot.Instance.Events.Unsubscribe<HitBallSpawnContext>(OnHitBallSpawnContext);
        }

        /// <summary>
        /// BatContactDetector/HitOrchestrator에서 HitBallSpawnContext가 발행되면
        /// 타구 시작 위치를 저장하고 추적을 시작한다.
        /// </summary>
        private void OnHitBallSpawnContext(HitBallSpawnContext ctx)
        {
            _launchPos = ctx.baseContext.spawnPosition;
            _hasLaunchPos = true;

            _trackingHit = true;
            _isFair = false;
            _landedInFoul = false;
            _isHomeRun = false;
            _caughtInAir = false;
            _requestSent = false;
            _elapsed = 0f;
        }

        private void FixedUpdate()
        {
            if (!_trackingHit)
                return;

            _elapsed += Time.fixedDeltaTime;
            if (_elapsed > maxHitDuration && !_requestSent)
            {
                // 타구가 너무 오래 지속되면 대충 페어로 보고 종료
                SendHitResultRequest(distance: 0f);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_trackingHit)
                return;

            var ball = other.GetComponentInParent<BallCore>();
            if (ball == null || ball.State.Phase != BallPhase.Hit)
                return;

            Vector3 hitPos = ball.State.Position;

            // 1) 홈런 존
            if (homeRunVolume != null && other == homeRunVolume)
            {
                _isHomeRun = true;
                _isFair = true;      // 홈런은 무조건 페어로 처리
                _trackingHit = false;
                SendHitResultRequest(CalcDistance(hitPos));
            }
            // 2) 수비 글러브 캐치 (공중에서)
            else if (other.CompareTag(fielderGloveTag))
            {
                _caughtInAir = true;
                _isFair = true;      // 단순화: 파울 존 글러브는 나중에 필요하면 분리
                _trackingHit = false;
                SendHitResultRequest(CalcDistance(hitPos));
            }
            // 3) 땅과 충돌
            else if (groundCollider != null && other == groundCollider)
            {
                bool inFair = IsPointInsideCollider(hitPos, fairTerritoryVolume);
                bool inFoulL = IsPointInsideCollider(hitPos, foulTerritoryVolumeLeft);
                bool inFoulR = IsPointInsideCollider(hitPos, foulTerritoryVolumeRight);

                if (inFair)
                {
                    _isFair = true;
                    _landedInFoul = false;
                }
                else if (inFoulL || inFoulR)
                {
                    _isFair = false;
                    _landedInFoul = true;
                }
                else
                {
                    // 아무 볼륨에도 안 들어가면 일단 페어로 보거나, 옵션에 맞게 조정 가능
                    _isFair = true;
                    _landedInFoul = false;
                }

                _trackingHit = false;
                SendHitResultRequest(CalcDistance(hitPos));
            }
        }

        private float CalcDistance(Vector3 endPos)
        {
            if (!_hasLaunchPos)
                return 0f;

            var a = _launchPos;
            a.y = 0f;
            var b = endPos;
            b.y = 0f;
            return Vector3.Distance(a, b);
        }

        private void SendHitResultRequest(float distance)
        {
            if (_requestSent)
                return;

            _requestSent = true;

            var req = new HitResultRequest
            {
                IsFair = _isFair,
                IsHomeRun = _isHomeRun,
                BallCaughtInAir = _caughtInAir,
                LandedInFoulTerritory = _landedInFoul,
                Distance = distance
            };

            GameRoot.Instance.Events.Publish(req);
        }

        private static bool IsPointInsideCollider(Vector3 point, Collider col)
        {
            if (col == null) return false;
            return col.bounds.Contains(point); // 간단한 AABB 기반 체크
        }
    }
}
