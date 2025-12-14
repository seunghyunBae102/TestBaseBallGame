using Bash.Core.Data;
using Bash.Core.GamePlay;
using Bash.Core.GamePlay.Environment;
using Bash.Core.Unit;
using Bash.Framework.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Bash.Framework.Managers
{
    public class RosterManager : ActorCompo
    {
        [SerializeField] private TeamDataSO _homeTeam;
        [SerializeField] private TeamDataSO _awayTeam;

        private ResourceManager _resourceMgr;

        // 필드 유닛 관리
        private List<UnitPawn> _activeFielders = new List<UnitPawn>();
        private UnitPawn _currentPitcher;
        private UnitPawn _currentBatter;

        // 타순 관리
        private Dictionary<TeamDataSO, int> _battingIdx = new Dictionary<TeamDataSO, int>();

        protected override void OnInit()
        {
            _resourceMgr = GameRoot.Instance.GetManager<ResourceManager>();
            if (_homeTeam) _battingIdx[_homeTeam] = 0;
            if (_awayTeam) _battingIdx[_awayTeam] = 0;
        }

        // --- Async API (MatchFlowManager 호출용) ---

        public async Task PrepareInningAsync(bool isTop)
        {
            // ... (기존 초기화) ...
            TeamDataSO defenseTeam = isTop ? _homeTeam : _awayTeam;

            // 1. 투수 생성
            if (defenseTeam.Pitcher != null)
            {
                var p = await SpawnUnitAsync(defenseTeam.Pitcher.Data, new Vector3(0, 0, -18.44f), Quaternion.identity, !isTop);
                AssignPosition(p, EFieldPosition.Pitcher);
            }

            // 2. 1루수 생성
            var fieldMgr = GameRoot.Instance.GetManager<FieldManager>();
            if (fieldMgr != null && defenseTeam.FirstBaseman != null)
            {
                var base1 = fieldMgr.CurrentField.GetBase(BaseNode.EBaseType.First);
                var p = await SpawnUnitAsync(defenseTeam.FirstBaseman.Data, base1.GetCatchPosition(), Quaternion.identity, !isTop);

                if (p)
                {
                    p.LookAt(Vector3.zero);
                    _activeFielders.Add(p);
                    // [중요] 포지션 할당
                    AssignPosition(p, EFieldPosition.FirstBase);
                }
            }

            // ... (유격수, 2루수, 3루수, 외야수 등 동일하게 AssignPosition 호출) ...
        }

        // [New] 헬퍼 메서드
        private void AssignPosition(UnitPawn pawn, EFieldPosition posID)
        {
            if (pawn == null) return;
            var ctrl = pawn.GetComponent<FielderController>();
            if (ctrl != null)
            {
                ctrl.PositionID = posID;
            }
        }


        public async Task PrepareNextBatterAsync(bool isTop)
        {
            TeamDataSO attackTeam = isTop ? _awayTeam : _homeTeam;
            int idx = _battingIdx[attackTeam];

            if (attackTeam.BattingOrder.Count > 0)
            {
                var data = attackTeam.BattingOrder[idx % attackTeam.BattingOrder.Count];

                if (_currentBatter) Destroy(_currentBatter.gameObject);

                Vector3 boxPos = new Vector3(-1f, 0, 0);
                _currentBatter = await SpawnUnitAsync(data.Data, boxPos, Quaternion.LookRotation(Vector3.back), isTop);

                _battingIdx[attackTeam] = idx + 1;
            }
        }

        // --- Core Spawning Logic ---
        private async Task<UnitPawn> SpawnUnitAsync(PlayerDTO data, Vector3 pos, Quaternion rot, bool isHome)
        {
            if (_resourceMgr == null || data == null) return null;

            // 1. 어드레서블 로드
            GameObject prefab = await _resourceMgr.LoadModelAsync(data.ModelID);
            if (prefab == null) return null;

            // 2. 생성
            GameObject go = Instantiate(prefab, pos, rot);

            // 3. 연결
            go.GetComponent<ActorNode>()?.AttachTo(GameRoot.Instance.WorldRoot);

            // 4. 데이터 주입 (Pawn 초기화)
            var pawn = go.GetComponent<UnitPawn>();
            if (pawn) pawn.Initialize(data, isHome);

            return pawn;
        }


        private void ClearField()
        {
            if (_currentPitcher) Destroy(_currentPitcher.gameObject);
            foreach (var f in _activeFielders) Destroy(f.gameObject);
            _activeFielders.Clear();
        }
    }
}