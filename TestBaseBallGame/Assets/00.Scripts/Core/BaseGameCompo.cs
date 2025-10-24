using System.Collections.Generic;
using UnityEngine;

public class BaseGameCompo : MonoBehaviour
{
    public List<string> Mytag; // Unity tag�� 1���ۿ� �ȵǴ�;; = Scat  // �׷��⿡ ���Ե� ���� tag�� ���� �� �ִ� �ý���!

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
