using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SurfaceProps
{
    public string key;            // tag/PhysicMaterial.name ��
    [Range(0, 1)] public float restitution = 0.35f; // e: ź�����(�ܵ� ����)
    [Range(0, 2)] public float friction = 0.6f;   // ��: �浹 ���� ����
    [Range(0, 0.2f)] public float rollingResistance = 0.04f; // ��_r: ���� ����
    [Range(0, 1)] public float spinDamping = 0.1f;  // ȸ�� ����
}

[CreateAssetMenu(menuName = "Baseball/Physics/SurfaceCatalog")]
public class SurfaceCatalogSO : ScriptableObject
{
    public SurfaceProps defaultProps;
    public List<SurfaceProps> list = new();
    public SurfaceProps GetFor(Collider col)
    {
        var tag = col.tag;
        foreach (var p in list) if (p.key == tag) return p;
        return defaultProps;
    }
}
