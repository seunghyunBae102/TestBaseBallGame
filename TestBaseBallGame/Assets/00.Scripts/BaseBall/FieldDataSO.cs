using UnityEngine;

namespace Bash.Core.Data
{
    [CreateAssetMenu(fileName = "FieldData", menuName = "Bash/Field Data")]
    public class FieldDataSO : ScriptableObject
    {
        [Header("Foul Lines")]
        [Tooltip("중앙(0도) 기준 파울 라인 각도 (일반적으로 45도)")]
        public float FoulLineAngle = 45.0f;

        [Header("Home Run Fence")]
        [Tooltip("중견수 방향(Center) 펜스 거리 (m)")]
        public float HomeRunDistanceCenter = 120.0f;

        [Tooltip("좌우 폴대(Side) 펜스 거리 (m)")]
        public float HomeRunDistanceSide = 98.0f;
    }
}