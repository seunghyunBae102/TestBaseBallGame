using UnityEngine;
using Bash.Framework.Core;
using Bash.Framework.Core.Events;

namespace Bash.Core.Rules
{
    public class GameRuleManager : ActorCompo
    {
        [Header("Game State")]
        public int Score = 0;
        public int StrikeCount = 0;
        public int BallCount = 0;
        public int OutCount = 0;
        public int Inning = 1;

        protected override void OnInit()
        {
            // 이벤트 구독 (사건 청취)
            Events.Subscribe<BallReachCatcherEvent>(OnBallReachCatcher);
            Events.Subscribe<BallHitEvent>(OnBallHit);

            // 초기 상태 알림
            BroadcastState();
        }

        protected override void OnDestroy()
        {
            // 구독 해제 (메모리 누수 방지)
            Events.Unsubscribe<BallReachCatcherEvent>(OnBallReachCatcher);
            Events.Unsubscribe<BallHitEvent>(OnBallHit);
        }

        // --- A. 포수 미트 도달 시 판정 (스트라이크/볼) ---
        private void OnBallReachCatcher(BallReachCatcherEvent evt)
        {
            // 1. 스윙을 했는가? -> 헛스윙은 무조건 스트라이크
            if (evt.IsSwung)
            {
                ApplyJudgement(JudgementEvent.Type.Strike, "SWING & MISS!");
                return;
            }

            // 2. 존 안에 들어왔는가? -> 루킹 스트라이크
            if (evt.IsStrikeZone)
            {
                ApplyJudgement(JudgementEvent.Type.Strike, "LOOKING STRIKE!");
            }
            else
            {
                ApplyJudgement(JudgementEvent.Type.Ball, "BALL");
            }
        }

        // --- B. 타격 발생 시 판정 (안타/아웃) ---
        private void OnBallHit(BallHitEvent evt)
        {
            // *단순화된 로직*
            // 실제로는 수비수 AI가 잡았는지, 파울 라인 안쪽인지 계산해야 함.
            // 여기서는 거리에 따라 안타/홈런을 가상 판정합니다.

            float distance = evt.LandingPoint.z; // Z축 전방 거리 가정

            if (distance > 100f) // 100m 이상 홈런
            {
                AddScore(1);
                ApplyJudgement(JudgementEvent.Type.HomeRun, "HOME RUN!!");
                ResetCount();
            }
            else if (distance > 30f) // 30m 이상 안타
            {
                ApplyJudgement(JudgementEvent.Type.Hit, "HIT!");
                ResetCount();
                // (주자 로직은 추후 추가)
            }
            else // 땅볼/플라이 아웃
            {
                ApplyJudgement(JudgementEvent.Type.Out, "OUT (Ground Ball)");
                AddOut();
            }
        }

        // --- C. 판정 적용 (핵심: 스킬 개입 가능성 열어두기) ---
        //public void ApplyJudgement(JudgementEvent.Type type, string msg)
        //{
        //    // TODO: 여기서 'PreJudgementEvent'를 발행하면 
        //    // "10% 확률로 스트라이크를 볼로 만드는 스킬" 등이 개입하여 type을 바꿀 수 있음.

        //    Debug.Log($"[Ref] JUDGE: {msg}");
        //    Events.Publish(new JudgementEvent { Result = type, Message = msg });

        //    switch (type)
        //    {
        //        case JudgementEvent.Type.Strike:
        //            AddStrike();
        //            break;
        //        case JudgementEvent.Type.Ball:
        //            AddBall();
        //            break;
        //            // Hit, HomeRun, Out 등은 위에서 이미 처리함 (ResetCount 등)
        //    }
        //}
        // [GameRuleManager.cs] ApplyJudgement 메서드 수정
        public void ApplyJudgement(JudgementEvent.Type type, string msg)
        {
            // 1. 예비 판정 이벤트 발행 (스킬들에게 기회를 줌)
            var preEvent = new PreJudgementEvent { OriginalResult = type, FinalResult = type };
            Events.Publish(preEvent);

            // 2. 스킬이 개입했다면 결과 변경
            if (preEvent.IsModified)
            {
                type = preEvent.FinalResult;
                msg = $"[Skill] Result Changed to {type}!";
            }

            // 3. 최종 확정 (기존 로직 수행)
            Debug.Log($"[Ref] JUDGE: {msg}");
            Events.Publish(new JudgementEvent { Result = type, Message = msg });

            switch (type)
            {
               case JudgementEvent.Type.Strike:
                    AddStrike();
                    break;
                case JudgementEvent.Type.Ball:
                    AddBall();
                    break;
                    // Hit, HomeRun, Out 등은 위에서 이미 처리함 (ResetCount 등)
            }
        }
        // --- D. 카운트 로직 ---

        private void AddStrike()
        {
            StrikeCount++;
            if (StrikeCount >= 3)
            {
                ApplyJudgement(JudgementEvent.Type.Out, "STRIKE OUT!");
                AddOut();
            }
            else
            {
                BroadcastState();
            }
        }

        private void AddBall()
        {
            BallCount++;
            if (BallCount >= 4)
            {
                ApplyJudgement(JudgementEvent.Type.Hit, "BASE ON BALLS"); // 볼넷은 안타 취급(출루)
                ResetCount();
            }
            else
            {
                BroadcastState();
            }
        }

        private void AddOut()
        {
            OutCount++;
            ResetCount(); // 타자 교체

            if (OutCount >= 3)
            {
                Debug.Log($"[Ref] Inning Change! End of Inning {Inning}");
                Inning++;
                OutCount = 0;
                // 공수 교대 로직 호출
            }

            BroadcastState();
        }

        private void AddScore(int amount)
        {
            Score += amount;
            BroadcastState();
        }

        private void ResetCount()
        {
            StrikeCount = 0;
            BallCount = 0;
            BroadcastState();
        }

        private void BroadcastState()
        {
            Events.Publish(new GameStateEvent
            {
                Score = Score,
                Strike = StrikeCount,
                Ball = BallCount,
                Out = OutCount
            });
        }
    }
}