using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SurfaceProps
{
    public string key;            // tag/PhysicMaterial.name 등
    [Range(0, 1)] public float restitution = 0.35f; // e: 탄성계수(잔디 낮게)
    [Range(0, 2)] public float friction = 0.6f;   // μ: 충돌 접선 감속
    [Range(0, 0.2f)] public float rollingResistance = 0.04f; // μ_r: 구름 저항
    [Range(0, 1)] public float spinDamping = 0.1f;  // 회전 감쇠
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
