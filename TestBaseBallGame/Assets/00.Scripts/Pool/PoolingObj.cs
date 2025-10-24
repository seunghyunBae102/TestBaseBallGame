using UnityEngine;

public class PoolingObj : GetCompoParentSample<PoolingObj>
{
    protected PoolManager _mgr;
    protected string _id;
    public void Bind(PoolManager mgr, string id) { _mgr = mgr; _id = id; }

    public void Return() => _mgr?.Despawn(_id, this);

    // ��: ��ƼŬ ������ �ڵ� �ݳ�
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
