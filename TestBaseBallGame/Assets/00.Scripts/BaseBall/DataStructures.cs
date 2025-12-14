using System.Collections.Generic;
using UnityEngine;
using Bash.Core.Unit; // EStatType 참조

namespace Bash.Core.Data
{
    // [스탯 설정 규격]
    [System.Serializable]
    public struct StatConfig
    {
        public EStatType Type;
        [Range(0, 150)] public float Value;
    }

    // [선수 데이터 전송 객체 (DTO)] - 서버/JSON 통신 규격
    [System.Serializable]
    public class PlayerDTO
    {
        public string ID;           // 고유 ID (예: "PL_001")
        public string Name;         // 이름

        [Header("Resource Key")]
        public string ModelID;      // 어드레서블 주소 (예: "Pitcher_Default")

        [Header("Stats")]
        public List<StatConfig> BaseStats;
    }

    public enum EFieldPosition
    {
        Pitcher, Catcher,
        FirstBase, SecondBase, ThirdBase, ShortStop,
        LeftField, CenterField, RightField,
        None
    }
}