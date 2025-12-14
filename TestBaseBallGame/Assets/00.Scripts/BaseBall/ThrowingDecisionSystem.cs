using UnityEngine;
using Bash.Framework.Core;
using Bash.Framework.Managers;
using Bash.Core.GamePlay.Environment;
using Bash.Core.Unit;
using Bash.Core.Rules;
using Bash.Framework.Utils; // Ballistics

namespace Bash.Core.GamePlay.AI
{
    public class ThrowingDecisionSystem : ActorCompo
    {
        private FieldManager _fieldMgr;
        private OutLogicManager _outLogic;

        [Header("Weights")]
        [SerializeField] private float _bufferTime = 0.5f; // 최소 이 정도는 여유가 있어야 던짐 (초)
        [SerializeField] private float _forceOutBonus = 1.0f; // 포스 아웃이면 가산점 (초 단위 환산)

        protected override void OnInit()
        {
            _fieldMgr = GameRoot.Instance.GetManager<FieldManager>();
            _outLogic = GameRoot.Instance.GetManager<OutLogicManager>();
        }

        /// <summary>
        /// 수비수가 공을 잡은 위치에서 가장 좋은 송구 타겟을 계산하여 반환
        /// </summary>
        public BaseNode GetBestThrowTarget(Vector3 fielderPos, float throwingPower)
        {
            if (_fieldMgr == null || _outLogic == null) return null;

            BaseNode bestBase = null;
            float bestScore = -999f;

            // 홈 -> 3루 -> 2루 -> 1루 순으로 검사 (선행 주자 우선 원칙)
            // 배열 순서: Home(0), 1B(1), 2B(2), 3B(3) 이므로 역순 혹은 중요도 순으로 순회
            var checkOrder = new BaseNode.EBaseType[] {
                BaseNode.EBaseType.Home,
                BaseNode.EBaseType.Third,
                BaseNode.EBaseType.Second,
                BaseNode.EBaseType.First
            };

            foreach (var baseType in checkOrder)
            {
                float score = CalculateScore(baseType, fielderPos, throwingPower);

                // 유효한 아웃 기회이고, 기존 것보다 점수가 높으면 교체
                if (score > bestScore && score > 0)
                {
                    bestScore = score;
                    bestBase = _fieldMgr.CurrentField.GetBase(baseType);
                }
            }

            // 아웃 시킬 곳이 없으면? (All Safe 예상)
            // 보통은 1루로 던지거나, 리드하고 있는 선행 주자를 묶기 위해 해당 베이스로 던짐.
            // 여기선 null을 리턴하면 호출자가 기본값(1루)을 쓰도록 함.
            return bestBase;
        }

        private float CalculateScore(BaseNode.EBaseType targetBaseType, Vector3 throwOrigin, float throwSpeed)
        {
            var targetBase = _fieldMgr.CurrentField.GetBase(targetBaseType);

            // 1. 해당 베이스로 달리는 주자 찾기
            // 타겟이 1루 -> 타자주자
            // 타겟이 2루 -> 1루주자 ...
            UnitPawn runner = GetRunnerHeadingTo(targetBaseType);

            if (runner == null) return -999f; // 잡을 주자가 없음

            // 2. 거리 계산
            float distBall = Vector3.Distance(throwOrigin, targetBase.GetCatchPosition());
            float distRunner = Vector3.Distance(runner.transform.position, targetBase.GetTouchPosition());

            // 3. 시간 계산 (Time = Distance / Speed)
            float ballTime = distBall / throwSpeed; // 송구 시간 (준비 동작 포함해야 하나 여기선 단순화)

            float runnerSpeed = runner.Stat ? runner.Stat.GetStat(EStatType.Speed) : 5.0f;
            // 스탯 100 기준 약 8~9m/s. 단순화하여 계산
            float actualRunnerSpeed = 4.0f + (runnerSpeed * 0.05f);
            float runnerTime = distRunner / actualRunnerSpeed;

            // 4. 여유 시간 (Delta)
            // 양수면 공이 먼저 도착, 음수면 주자가 먼저 도착 (세이프)
            float timeDelta = runnerTime - ballTime;

            // 아웃 불가능하면 낮은 점수
            if (timeDelta < _bufferTime) return -999f;

            // 5. 상황 가산점 (포스 아웃 우대)
            // 땅볼 가정(true). 실제로는 FieldingDirector가 뜬공 여부 알려줘야 함.
            var outType = _outLogic.GetOutType(targetBaseType, true);

            if (outType == EOutType.ForceOut)
            {
                timeDelta += _forceOutBonus; // 태그할 필요 없으니 더 유리함
            }
            else if (outType == EOutType.TagOut)
            {
                // 태그 아웃은 시간이 더 걸리므로 페널티
                timeDelta -= 0.5f;
            }
            else
            {
                return -999f; // 아웃 상황 아님 (이미 점유 중이거나 등등)
            }

            // 6. 베이스 가치 가산점 (선행 주자 잡는게 점수 높음)
            // Home(4점) > 3B(3점) > 2B(2점) > 1B(1점) - 임의 가중치
            float baseValue = 0f;
            switch (targetBaseType)
            {
                case BaseNode.EBaseType.Home: baseValue = 2.0f; break;
                case BaseNode.EBaseType.Third: baseValue = 1.5f; break;
                case BaseNode.EBaseType.Second: baseValue = 1.0f; break;
                case BaseNode.EBaseType.First: baseValue = 0.0f; break;
            }

            return timeDelta + baseValue;
        }

        private UnitPawn GetRunnerHeadingTo(BaseNode.EBaseType target)
        {
            var field = _fieldMgr.CurrentField;

            if (target == BaseNode.EBaseType.First)
            {
                // 1루로 뛰는 건 타자주자. 
                // 타자 주자를 추적하는 전역 변수가 필요하거나, 
                // 여기서는 간단히 "1루가 비어있지 않거나 타격 직후"를 가정해야 함.
                // 임시: 현재 씬의 '타자'를 찾거나, RunnerManager를 통해 가져와야 함.
                // 편의상: 1루가 비어있으면 타자가 뛰고 있다고 가정 (부정확할 수 있음)
                return null; // TODO: RunnerManager에서 "BatterRunner" 가져오기 구현 필요
            }
            else
            {
                // 2루로 뛰는 건 1루 주자
                int prevIdx = ((int)target - 1);
                if (prevIdx < 0) prevIdx = 3;

                var prevBase = field.GetBase((BaseNode.EBaseType)prevIdx);
                return prevBase.CurrentRunner;
            }
        }
    }
}