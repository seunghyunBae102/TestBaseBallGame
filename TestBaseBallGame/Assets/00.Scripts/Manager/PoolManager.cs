using UnityEngine;
using System.Collections.Generic;

public interface IPoolable
{              // 선택: 상태 리셋/훅
    void OnSpawned();
    void OnDespawned();
}

public class PoolManager : GetableManager, ILifeCycleable<GameManager>
{
    [SerializeField]
    protected PoolProfileSO[] _profiles;

    class Pool
    {
        public readonly Queue<PoolingObj> Idle = new();
        public readonly HashSet<PoolingObj> Busy = new();
        public Transform Root;
        public PoolProfileSO Profile;
    }

    readonly Dictionary<string, Pool> _pools = new();

    public void SetCompoParent(GameManager p) => Mom = p;

    public void Init()
    {
        foreach (var pf in _profiles) CreatePool(pf);
    }

    void CreatePool(PoolProfileSO pf)
    {
        var root = new GameObject($"Pool_{pf.Id}").transform;
        if (pf.DontDestroyInstances) DontDestroyOnLoad(root.gameObject);

        var pool = new Pool { Profile = pf, Root = root };
        _pools[pf.Id] = pool;

        // Prewarm
        for (int i = 0; i < Mathf.Max(0, pf.PrewarmCount); i++)
            pool.Idle.Enqueue(InstantiateInstance(pf, root));
    }

    protected PoolingObj InstantiateInstance(PoolProfileSO pf, Transform parent)
    {
        var go = Instantiate(pf.Prefab, parent);
        go.OnDespawned(); // 비활성화
        //var rt = go.GetComponent<ReturnToPool>();
        //if (!rt) rt = go.AddComponent<ReturnToPool>(); // 자동 반납용(선택)
        //rt.Bind(this, pf.Id);

        return go;
    }

    public PoolingObj Spawn(string id, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        if (!_pools.TryGetValue(id, out var pool)) return null;

        PoolingObj go = null;
        while (pool.Idle.Count > 0 && go == null)
        {
            go = pool.Idle.Dequeue();
            if (go == null) continue;
        }

        if (go == null)
        {
            if (!pool.Profile.Expandable && pool.Busy.Count >= pool.Profile.MaxCount)
                return null;
            go = InstantiateInstance(pool.Profile, pool.Root);
        }

        pool.Busy.Add(go);
        if (parent) go.transform.SetParent(parent, false);
        go.transform.SetPositionAndRotation(pos, rot);
        go.OnSpawned();
        go.GetComponent<IPoolable>()?.OnSpawned();
        return go;
    }

    public void Despawn(string id, PoolingObj go)
    {
        if (go == null) return;
        if (!_pools.TryGetValue(id, out var pool)) { Destroy(go); return; }

        if (go.TryGetComponent<IPoolable>(out var p)) p.OnDespawned();
        go.OnDespawned();
        go.transform.SetParent(pool.Root, false);
        pool.Busy.Remove(go);
        pool.Idle.Enqueue(go);
    }

    public void Dispose()
    {
        _pools.Clear();
    }

}
//public class ReturnToPool : MonoBehaviour
//{
//    protected PoolManager _mgr;
//    protected string _id;
//    public void Bind(PoolManager mgr, string id) { _mgr = mgr; _id = id; }

//    public void Return() => _mgr?.Despawn(_id, gameObject);

//    // 예: 파티클 끝나면 자동 반납
//    void OnParticleSystemStopped() => Return();
//}

