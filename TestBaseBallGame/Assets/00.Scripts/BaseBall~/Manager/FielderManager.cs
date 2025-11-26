// Scripts/Game/Baseball/Match/Fielder/FielderManager.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 필드 위의 모든 수비수를 관리하는 매니저.
/// - 시작 시 FielderActor를 자동 검색하거나,
/// 인스펙터에서 직접 등록해도 된다.
/// - "가장 가까운 수비수" 같은 쿼리를 제공.
/// </summary>
public class FielderManager : ManagerBase
{
    [SerializeField] private List<FielderActor> _fielders = new();

    protected override void Initialize()
    {
        base.Initialize();

        if (_fielders.Count == 0)
        {
            // 씬에서 자동 검색
            _fielders.AddRange(FindObjectsOfType<FielderActor>());
        }
    }

    public IReadOnlyList<FielderActor> Fielders => _fielders;

    public FielderActor GetClosestFielder(Vector3 position, TeamSide defenseTeam)
    {
        FielderActor best = null;
        float bestSqr = float.MaxValue;

        foreach (var f in _fielders)
        {
            if (f == null) continue;
            if (f.TeamSide != defenseTeam) continue;

            float sqr = (f.CachedTransform.position - position).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = f;
            }
        }

        return best;
    }
}
