using System.Collections.Generic;
using UnityEngine;
using Bash.Framework.Core;
using Bash.Framework.Core.Events;
using Bash.Core.Ball;
using Bash.Core.Data;
using Bash.Framework.Managers;
using Bash.Core.GamePlay.Environment;

namespace Bash.Core.GamePlay
{
    public class FieldingDirector : ActorCompo
    {
        private List<FielderController> _fielders = new List<FielderController>();
        private FieldManager _fieldMgr;

        protected override void OnInit()
        {
            _fieldMgr = GameRoot.Instance.GetManager<FieldManager>();

            // 라운드 시작될 때 수비수 목록 갱신
            Events.Subscribe<RoundStartEvent>(RefreshFielders);
            // 공이 맞았을 때 지휘 시작
            Events.Subscribe<BallHitEvent>(OnBallHit);
        }

        protected override void OnDestroy()
        {
            Events.Unsubscribe<RoundStartEvent>(RefreshFielders);
            Events.Unsubscribe<BallHitEvent>(OnBallHit);
        }

        private void RefreshFielders(RoundStartEvent evt)
        {
            _fielders.Clear();
            // 현재 씬의 모든 수비수 컨트롤러 수집
            // (주의: RosterManager가 생성한 애들만 가져오는 게 좋음)
            var allPawns = GameRoot.Instance.FindNodes<FielderPawn>();
            foreach (var pawn in allPawns)
            {
                var ctrl = pawn.GetComponent<FielderController>();
                if (ctrl != null) _fielders.Add(ctrl);
            }
        }

        private void OnBallHit(BallHitEvent evt)
        {
            if (_fielders.Count == 0) return;

            // 1. 공 찾기
            var ball = GameRoot.Instance.FindNode<BallNode>();
            if (ball == null) return;

            // 2. Chaser(추격자) 선정: 낙구 지점과 가장 가까운 수비수
            FielderController bestChaser = null;
            float minDesc = float.MaxValue;

            foreach (var f in _fielders)
            {
                float dist = Vector3.Distance(f.transform.position, evt.LandingPoint);
                if (dist < minDesc)
                {
                    minDesc = dist;
                    bestChaser = f;
                }
            }

            // 3. 역할 분배
            foreach (var fielder in _fielders)
            {
                if (fielder == bestChaser)
                {
                    // "너는 공 잡아!"
                    fielder.OrderChase(ball);
                }
                else
                {
                    // "너는 빈 베이스 채워!"
                    AssignCoverTask(fielder, bestChaser);
                }
            }
        }

        private void AssignCoverTask(FielderController fielder, FielderController chaser)
        {
            if (_fieldMgr == null || _fieldMgr.CurrentField == null) return;
            var field = _fieldMgr.CurrentField;

            // 간단한 베이스 커버 로직
            switch (fielder.PositionID)
            {
                case EFieldPosition.FirstBase:
                    // 1루수가 공 잡으러 안 갔으면 1루 지킴
                    fielder.OrderCover(field.GetBase(BaseNode.EBaseType.First));
                    break;

                case EFieldPosition.SecondBase:
                    // 2루수: 1루수가 공 잡으러 갔으면(chaser가 1루수) -> 1루 커버
                    // 아니면 -> 2루 커버
                    if (chaser.PositionID == EFieldPosition.FirstBase)
                        fielder.OrderCover(field.GetBase(BaseNode.EBaseType.First));
                    else
                        fielder.OrderCover(field.GetBase(BaseNode.EBaseType.Second));
                    break;

                case EFieldPosition.ShortStop:
                    // 유격수: 2루 커버
                    fielder.OrderCover(field.GetBase(BaseNode.EBaseType.Second));
                    break;

                case EFieldPosition.ThirdBase:
                    fielder.OrderCover(field.GetBase(BaseNode.EBaseType.Third));
                    break;

                case EFieldPosition.Catcher:
                    fielder.OrderCover(field.GetBase(BaseNode.EBaseType.Home));
                    break;

                case EFieldPosition.Pitcher:
                    // 투수는 1루수가 공 잡으러 갔을 때 1루 백업이 정석
                    if (chaser.PositionID == EFieldPosition.FirstBase)
                        fielder.OrderCover(field.GetBase(BaseNode.EBaseType.First));
                    else
                        fielder.OrderIdle();
                    break;

                default: // 외야수
                    fielder.OrderIdle(); // 일단 대기
                    break;
            }
        }
    }
}