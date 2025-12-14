using UnityEngine;
using Bash.Framework.Core;
using Bash.Core.Data;

namespace Bash.Core.GamePlay.Environment
{
    // 판정 결과 타입
    public enum EZoneType { Fair, Foul, HomeRun }

    public class FieldZoneManager : ActorCompo
    {
        [SerializeField] private FieldDataSO _fieldData;

        // 좌표를 입력받아 구역 타입 반환
        public EZoneType GetZoneType(Vector3 ballPos)
        {
            // 데이터가 없으면 기본적으로 페어(InPlay) 처리
            if (_fieldData == null) return EZoneType.Fair;

            // 1. 각도 계산 (Foul Check)
            // 야구장은 Z축(Forward)이 투수판/2루 방향이라고 가정
            Vector3 dir = ballPos;
            dir.y = 0; // 높이 무시

            // 정면(Z축)과 공 위치 사이의 각도 계산
            float angle = Vector3.Angle(Vector3.forward, dir);

            // 각도가 설정된 파울 라인보다 크면 파울
            if (angle > _fieldData.FoulLineAngle)
            {
                return EZoneType.Foul;
            }

            // 2. 홈런 체크 (거리 계산)
            float distance = dir.magnitude; // 원점(홈)에서의 거리

            // 펜스 거리 보간 (중앙과 측면 사이 거리 계산)
            // 각도 비율(t): 0(중앙) ~ 1(폴대)
            float t = Mathf.Clamp01(angle / _fieldData.FoulLineAngle);

            // 부채꼴 타원형 구장을 가정하여 거리 보간
            float fenceDist = Mathf.Lerp(_fieldData.HomeRunDistanceCenter, _fieldData.HomeRunDistanceSide, t);

            if (distance > fenceDist)
            {
                return EZoneType.HomeRun;
            }

            // 파울도 아니고 홈런도 아니면 페어(인플레이)
            return EZoneType.Fair;
        }
    }
}