using UnityEngine;

[CreateAssetMenu(menuName = "Pooling/Pool Profile")]
public class PoolProfileSO : ScriptableObject
{
    public string Id;                 // "Ball", "HitSpark", "AnnouncerVoice" ��
    public PoolingObj Prefab;         // �Ǵ� AssetReferenceGameObject (Addressables)
    public int PrewarmCount = 8;
    public int MaxCount = 64;
    public bool Expandable = true;
    public bool DontDestroyInstances = true; // �� ��ȯ �Ŀ��� �����ϰ� ������
}

