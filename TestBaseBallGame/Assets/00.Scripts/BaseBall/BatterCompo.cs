using Bash.Core.Ball;
using Bash.Framework.Core;
using Bash.Framework.Core.Events;
using UnityEngine;

namespace Bash.Core.GamePlay
{
    public class BatterCompo : ActorCompo
    {
        [Header("Hit Box Settings")]
        [SerializeField] private float _sweetSpotRadius = 0.5f; // 타격 유효 거리
        [SerializeField] private float _powerMultiplier = 1.5f; // 반발 계수

        // 현재 날아오는 공을 추적 (게임 매니저가 알려주거나, Raycast로 감지)
        private BallNode _targetBall;

        // UI에서 드래그가 끝났을 때 호출
        // swingVector: 드래그 방향 (공이 날아갈 방향에 영향)
        // timingAccuracy: 0.0 ~ 1.0 (1.0이면 정타, 0.0이면 빗맞음)
        public bool AttemptSwing(Vector3 swingVector, float timingAccuracy)
        {
            // 1. 공 찾기 (간단히 WorldRoot에서 BallNode 검색, 실제론 캐싱 필요)
            if (_targetBall == null)
            {
                _targetBall = GameRoot.Instance.FindNode<BallNode>();
            }
            if (_targetBall == null) return false;

            // 2. 거리 체크 (히트박스 판정)
            float dist = Vector3.Distance(transform.position, _targetBall.transform.position);

            // 공이 너무 멀면 헛스윙
            if (dist > _sweetSpotRadius)
            {
                Debug.Log("헛스윙! (거리 멀음)");
                return false;
            }

            // 3. 타격 성공! -> 전략 교체 (Hit Logic)
            HitBall(swingVector, timingAccuracy);
            return true;
        }
        //public bool AttemptSwing(Vector3 swingVector, float timingAccuracy)
        //{
        //    // ... (기존 공 찾기 코드) ...

        //    // 거리 체크 (헛스윙)
        //    if (dist > _sweetSpotRadius)
        //    {
        //        Debug.Log("헛스윙!");
        //        // 심판에게 보고: "휘둘렀는데 못 맞췄음"
        //        GameRoot.Instance.Events.Publish(new BallReachCatcherEvent
        //        {
        //            IsStrikeZone = false, // 존 여부 상관없이
        //            IsSwung = true        // 스윙 했음 -> 스트라이크
        //        });
        //        return false;
        //    }

        //    // 타격 성공 시 -> HitBall 호출 (기존 코드)
        //    HitBall(swingVector, timingAccuracy);
        //    return true;
        //}

        private void HitBall(Vector3 swingDir, float accuracy)
        {
            Debug.Log("타격 성공!");

            // A. 공의 물리 컴포넌트 가져오기
            var phys = _targetBall.GetCompo<BallPhysicsCompo>();

            // B. 타구 속도 계산 (물리 공식을 흉내 낸 게임 로직)
            // 투구 속도(반발력) + 스윙 파워
            Vector3 incomingVel = _targetBall.Velocity;
            float reboundPower = incomingVel.magnitude * 0.4f; // 오는 공 속도의 40% 이용
            float swingPower = 20.0f * _powerMultiplier * accuracy; // 스윙 자체 파워

            // 최종 날아갈 방향: 스윙 방향 + 약간의 위쪽 보정 (탄도)
            Vector3 finalDir = (swingDir.normalized + Vector3.up * 0.3f).normalized;
            Vector3 finalVel = finalDir * (reboundPower + swingPower);

            // C. 공의 뇌를 '물리 모드'로 교체
            _targetBall.Velocity = finalVel;
            phys.SetStrategy(new HitPhysicsStrategy(gravity: 15f, drag: 0.2f));

            // (선택) 카메라 연출 이벤트 발행
            // Events.Publish(new CameraZoomEvent(_targetBall.transform));
        }
    }
}