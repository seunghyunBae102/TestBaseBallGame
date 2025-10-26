using UnityEngine;

public class PoolingObj : GetCompoParentSample<PoolingObj>
{
    protected PoolManager _poolManager;
    protected string _id;
    public void Bind(PoolManager mgr, string id) { _poolManager = mgr; _id = id; }

    public void Return() => _poolManager?.Despawn(_id, this);

    // 예: 파티클 끝나면 자동 반납
    void OnParticleSystemStopped() => Return();
    public virtual void OnSpawned()
    {
        gameObject.SetActive(true);
    }
    public virtual void OnSpawned(params object[] objs)
    {
        gameObject.SetActive(true);
    }

    public virtual void OnDespawned()
    {
        gameObject.SetActive(false);
    }
}
