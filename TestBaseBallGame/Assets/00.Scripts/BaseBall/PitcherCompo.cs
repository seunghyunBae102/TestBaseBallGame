using Bash.Core.Ball;
using Bash.Framework.Core;
using Bash.Framework.Managers;
using UnityEngine;
using Bash.Framework.Core.Events;

namespace Bash.Core.GamePlay
{
    public class PitcherCompo : ActorCompo
    {
        [Header("Settings")]
        [SerializeField] private GameObject _ballPrefab;
        [SerializeField] private Transform _handPos;

        // 실제 게임에선 스탯이나 스킬에서 가져올 값들
        [SerializeField] private float _pitchSpeed = 35.0f; // 약 126km/h
        [SerializeField] private float _curveAmount = 2.0f; // 휨 정도

        private BallNode _currentBall;
        // PitcherCompo.cs의 DoPitch 메서드 수정
        public void DoPitch(Vector3 targetZonePos)
        {
            // ... (기존 공 생성 코드) ...

            //var strategy = new PitchCurveStrategy(..., targetZonePos, ...);
            var strategy = new PitchCurveStrategy(
                _handPos.position,
                targetZonePos,
                _curveAmount,
                0.5f, // 위로 살짝 솟았다 떨어짐 (궤적 보정) 
                _pitchSpeed
            );
            // 전략에 "도착 시 실행할 행동"을 추가하거나, 
            // 간단히 TimerManager를 써서 도착 예정 시간에 이벤트를 발생시킵니다.
            // (PitchCurveStrategy는 시간이 결정되어 있으므로 타이머가 정확합니다)

            float duration = Vector3.Distance(_handPos.position, targetZonePos) / _pitchSpeed;

            // 타이머 매니저를 통해 "도착 보고" 예약
            var timer = GameRoot.Instance.GetManager<TimerManager>();
            timer.SetTimer(duration, () =>
            {
                // 공이 아직 '투구 모드'라면 (= 타자가 안 쳤다면)
                if (_currentBall != null && _currentBall.GetCompo<BallPhysicsCompo>().IsPitchingMode)
                {
                    // 심판에게 보고: "포수 미트 도착함. (스트라이크 존 체크 로직 필요)"
                    // 일단 테스트용으로 무조건 스트라이크 존이라고 가정
                    GameRoot.Instance.Events.Publish(new BallReachCatcherEvent
                    {
                        IsStrikeZone = true,
                        IsSwung = false
                    });

                    // 공 제거 or 리셋
                    Destroy(_currentBall.gameObject);
                }
            });

            var physics = _currentBall.GetCompo<BallPhysicsCompo>();
            physics.SetStrategy(strategy);
        }
        //// 외부(UI 버튼 등)에서 호출
        //public void DoPitch(Vector3 targetZonePos)
        //{
        //    // 1. 공 생성 (PoolManager를 쓰면 더 좋음, 일단 Instantiate)
        //    var go = Instantiate(_ballPrefab, _handPos.position, Quaternion.identity);
        //    _currentBall = go.GetComponent<BallNode>();

        //    // 가상 계층 연결 (중요!)
        //    _currentBall.AttachTo(GameRoot.Instance.WorldRoot);

        //    // 2. 전략 설정 (투구 모드)
        //    // 커브볼: 오른쪽으로 2.0만큼 휘어서 타겟으로 들어감
        //    var strategy = new PitchCurveStrategy(
        //        _handPos.position,
        //        targetZonePos,
        //        _curveAmount,
        //        0.5f, // 위로 살짝 솟았다 떨어짐 (궤적 보정) 
        //        _pitchSpeed
        //    );

        //    var physics = _currentBall.GetCompo<BallPhysicsCompo>();
        //    physics.SetStrategy(strategy);

        //    // 공 정보 세팅
        //    _currentBall.Velocity = Vector3.zero; // 초기 속도는 전략 내부에서 계산됨
        //}
    }
}