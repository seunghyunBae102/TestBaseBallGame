using System.Collections.Generic;
using UnityEngine;

public class BaseGameCompo : MonoBehaviour
{
    public List<string> Mytag; // Unity tag는 1개밖에 안되는;; = Scat  // 그렇기에 도입된 여러 tag를 가질 수 있는 시스템!

    public bool HasTag(string tag)
    {
        return Mytag.Contains(tag);
    }
    public bool AddTag(string tag)
    {
        if (!Mytag.Contains(tag))
        {
            Mytag.Add(tag);
            return true;
        }
        return false;
    }
    public bool RemoveTag(string tag)
    {
        if (Mytag.Contains(tag))
        {
            Mytag.Remove(tag);
            return true;
        }
        return false;
    }

}
