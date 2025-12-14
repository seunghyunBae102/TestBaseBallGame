using UnityEngine;

namespace Bash.Framework.Utils
{
    public static class Ballistics
    {
        // 목표까지 도달하기 위한 초기 속도 계산 (포물선)
        public static Vector3 CalculateVelocity(Vector3 start, Vector3 target, float duration, float gravity)
        {
            Vector3 displacement = target - start;
            Vector3 gravityVec = Vector3.down * gravity;
            return (displacement - (0.5f * gravityVec * duration * duration)) / duration;
        }

        // 스탯 기반 예상 비행 시간 계산
        public static float CalculateFlightTime(Vector3 start, Vector3 target, float speedStat)
        {
            float dist = Vector3.Distance(start, target);
            // 스탯 100일 때 45m/s 속도 가정
            float realSpeed = Mathf.Lerp(20f, 45f, speedStat / 100f);
            return dist / realSpeed;
        }

        // 정교함 스탯 기반 오차 적용
        public static Vector3 ApplyErrorToTarget(Vector3 target, float distance, float dexterityStat)
        {
            float errorRadius = Mathf.Lerp(3.0f, 0.05f, dexterityStat / 100f);
            errorRadius *= (1.0f + distance * 0.02f); // 거리에 비례해 오차 증가
            return target + (Random.insideUnitSphere * errorRadius);
        }
    }
}