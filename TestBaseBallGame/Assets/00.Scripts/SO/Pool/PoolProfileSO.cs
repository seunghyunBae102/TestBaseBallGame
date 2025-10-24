using UnityEngine;

[CreateAssetMenu(menuName = "Pooling/Pool Profile")]
public class PoolProfileSO : ScriptableObject
{
    public string Id;                 // "Ball", "HitSpark", "AnnouncerVoice" 등
    public PoolingObj Prefab;         // 또는 AssetReferenceGameObject (Addressables)
    public int PrewarmCount = 8;
    public int MaxCount = 64;
    public bool Expandable = true;
    public bool DontDestroyInstances = true; // 씬 전환 후에도 보존하고 싶으면
}

