using System;
using System.Collections.Generic;
using UnityEngine;

public static class BashUtils
{
    //public static Vector3 V2toV3(this Vector3 a, Vector2 v) // ExtendedMehtod!
    //{
    //    return new Vector3(v.x,0, v.y);
    //}
    public static Vector3 V2toV3(Vector2 v)
    {
        return new Vector3(v.x,0, v.y);
    }
    public static Vector3 V3X0Z(Vector3 v)
    {
        return new Vector3(v.x,0, v.z);
    }
}

[Serializable]
public class BashTuple<T1, T2>
{
    public T1 One;
    public T2 Two;
    public BashTuple(T1 one, T2 two)
    {
        One = one;
        Two = two;
    }

    public void SetOne(T1 one)
    {
        One = one;
    }
    public void SetTwo(T2 two)
    {
        Two = two;
    }
}
[Serializable]
public class BashList<T>
{
    public List<T> List;

    public BashList(List<T> list)
    {
        List = list;
    }
    public BashList()
    {
        List = new List<T>();
    }
}