using UnityEngine;
using Bash.Framework.Core;
using Bash.Core.GamePlay;

namespace Bash.Test
{
    public class DefenceTest : ActorNode
    {
        [SerializeField] private FielderPawn _testFielder; // 씬에 배치된 수비수
        [SerializeField] private Transform _targetPos; // 이동 목표 (빈 오브젝트)

        private FielderController _ai;

        protected override void Awake()
        {
            base.Awake();

            // 1. AI 생성 및 부착 (테스트용으로 즉석 생성)
            _ai = gameObject.AddComponent<FielderController>();

            // 2. 가상 계층 연결 (AI -> TestSceneNode)
            _ai.Init(this);

            // 3. 빙의 (AI가 수비수를 제어하기 시작)
            _ai.Possess(_testFielder);
        }

        private void Update()
        {
            // 우클릭하면 해당 위치로 이동 명령
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    _ai.OrderChase(hit.point);
                }
            }

            // Space바 누르면 미리 지정된 타겟으로 이동
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _ai.OrderChase(_targetPos.position);
            }
        }
    }
}