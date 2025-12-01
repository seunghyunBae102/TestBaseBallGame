using UnityEngine;
using Bash.Framework.Core;
using Bash.Core.GamePlay;

namespace Bash.Test
{
    // 테스트 씬 전용 컨트롤러 (WorldRoot에 붙여서 사용)
    public class BaseballTestScene : ActorNode
    {
        [Header("References")]
        [SerializeField] private PitcherCompo _pitcher;
        [SerializeField] private BatterCompo _batter;
        [SerializeField] private Transform _strikeZone; // 공이 날아올 목표 지점

        [Header("Debug Settings")]
        [SerializeField] private bool _autoSwing = false; // true면 자동 타격 (타이밍 테스트용)

        protected override void Awake()
        {
            base.Awake();

            // 테스트 편의를 위해 WorldRoot에 스스로를 등록하거나
            // GameRoot가 이미 있다면 AttachTo 호출
            if (GameRoot.Instance != null && GameRoot.Instance.WorldRoot != null)
            {
                AttachTo(GameRoot.Instance.WorldRoot);
            }
        }

        private void Update()
        {
            // 1. 투구 (P 키)
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("[Test] Pitching Start!");
                _pitcher.DoPitch(_strikeZone.position);
            }

            // 2. 타격 (Space 키)
            // 실제 UI 드래그 대신, 키보드를 누르면 "센터 방향"으로 스윙했다고 가정
            if (Input.GetKeyDown(KeyCode.Space) || _autoSwing)
            {
                // 가상 입력 데이터 생성
                Vector3 fakeDragVector = (Vector3.forward + Vector3.up * 0.5f).normalized; // 중견수 뒤쪽 방향
                float fakeTimingAccuracy = 1.0f; // 완벽한 타이밍 가정

                bool hit = _batter.AttemptSwing(fakeDragVector, fakeTimingAccuracy);

                if (hit) Debug.Log("<color=green>[Test] HIT!</color>");
                else Debug.Log("<color=red>[Test] MISS (Timing or Distance)</color>");

                // 자동 스윙일 경우 한 번만 실행되도록
                if (_autoSwing) _autoSwing = false;
            }
        }

        // 스트라이크 존 시각화
        private void OnDrawGizmos()
        {
            if (_strikeZone != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(_strikeZone.position, new Vector3(0.5f, 0.7f, 0.1f));
                Gizmos.DrawLine(_pitcher.transform.position, _strikeZone.position);
            }
        }
    }
}