using UnityEngine;
using Bash.Core.Ball;
using Bash.Core.GamePlay;

namespace Bash.Framework.Core.Events
{
    // 1. 투구 시작
    public struct PitchStartEvent { }

    // 2. 판정 요청 이벤트 (투/타 액터가 발생시킴)
    public struct BallReachCatcherEvent
    {
        public bool IsStrikeZone; // 스트라이크 존 통과 여부
        public bool IsSwung;      // 타자가 배트를 휘둘렀는가
    }

    public struct BallHitEvent
    {
        public Vector3 LandingPoint; // 공이 떨어진 위치 (안타/파울 판정용)
    }

    // 3. 판정 결과 이벤트 (룰 매니저가 발생시킴 - UI 갱신용)
    public struct JudgementEvent
    {
        public enum Type { Strike, Ball, Foul, Hit, Out, HomeRun }
        public Type Result;
        public string Message; // "STRIKE!", "OUT!" 등
    }

    // 4. 게임 상태 변경 (점수, 이닝)
    public struct GameStateEvent
    {
        public int Score;
        public int Strike;
        public int Ball;
        public int Out;
    }
    // [GameEvents.cs] 추가
    public class PreJudgementEvent
    {
        public JudgementEvent.Type OriginalResult; // 원래 판정 (예: Strike)
        public JudgementEvent.Type FinalResult;    // 변경될 판정 (스킬이 수정함)
        public bool IsModified = false;            // 스킬 개입 여부
    }
}