using System.Collections.Generic;
using UnityEngine;
using Bash.Framework.Core;
using Bash.Core.Data; // DTO 네임스페이스 참조

namespace Bash.Core.Unit
{
    // 스탯 종류 정의
    public enum EStatType
    {
        Strength,       // 힘 (송구 파워, 타구 속도)
        Stamina,        // 체력 (전력 질주 지속력)
        Speed,          // 속도 (이동 속도)
        Reflex,         // 순발력 (가속도, 회전 속도)
        Metabolism,     // 회복력 (스태미너 회복)
        Dexterity,      // 정교함 (송구 정확도)
        Throwing,       // 투척력 (송구 비거리)
        Perception,     // 인지력 (AI 반응 속도)
        Communication,  // 커뮤력 (충돌 회피)
        StressResist    // 침착성 (디버프 저항)
    }

    public class UnitStatCompo : ActorCompo
    {
        [Header("Runtime Status")]
        [Range(0f, 1f)] public float CurrentCondition = 1.0f; // 컨디션 (1.0 = 정상)
        public float CurrentStamina { get; private set; }

        // 실제 스탯 저장소
        private Dictionary<EStatType, float> _stats = new Dictionary<EStatType, float>();

        // [핵심] DTO 리스트를 받아 스탯 초기화 (RosterManager -> Pawn -> Here)
        public void ApplyStats(List<StatConfig> newStats)
        {
            if (newStats == null) return;

            _stats.Clear();
            foreach (var config in newStats)
            {
                _stats[config.Type] = config.Value;
            }

            // 스태미너 완충
            CurrentStamina = GetStatBase(EStatType.Stamina);
        }

        /// <summary>
        /// 컨디션이 적용된 최종 스탯 반환
        /// </summary>
        public float GetStat(EStatType type)
        {
            // 기본값 * 컨디션
            return _stats.TryGetValue(type, out float val) ? val * CurrentCondition : 0f;
        }

        /// <summary>
        /// 컨디션 영향 없는 순수 기본값 반환 (MaxStamina 확인용 등)
        /// </summary>
        public float GetStatBase(EStatType type)
        {
            return _stats.TryGetValue(type, out float val) ? val : 0f;
        }

        // --- 스태미너 로직 ---

        public void ConsumeStamina(float amount)
        {
            CurrentStamina = Mathf.Max(0, CurrentStamina - amount);
        }

        public override void OnTick(float dt)
        {
            // 자동 회복 (Metabolism 스탯 영향)
            float max = GetStatBase(EStatType.Stamina);
            if (CurrentStamina < max)
            {
                float metabolism = GetStat(EStatType.Metabolism);
                // 기본 회복 5 + (대사량 * 0.1)
                float recoveryRate = 5.0f + (metabolism * 0.1f);

                CurrentStamina = Mathf.Min(max, CurrentStamina + recoveryRate * dt);
            }
        }
    }
}