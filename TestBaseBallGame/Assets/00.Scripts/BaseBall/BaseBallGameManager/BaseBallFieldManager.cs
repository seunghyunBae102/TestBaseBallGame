using UnityEngine;
public struct FieldInfo
{
    public Vector3 Gravity;
    public float AirFriction;
    public Vector3 Wind;


}
public class BaseBallFieldManager : BaseBallGetableManager
{
    public FieldInfo FieldInfo;
}
