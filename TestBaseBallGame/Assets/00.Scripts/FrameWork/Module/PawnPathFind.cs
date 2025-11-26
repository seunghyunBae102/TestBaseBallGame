using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Bash.Framework.Core;

public class PawnPathFind : Module
{
    //대충 코앞에 있는 장애물 피해서 가는거 - 복잡한 미로는 풀지 못함.
    //아마 이거 재귀로 구현하거나 길게쏘고 이분탐색 조지면 보이는 범위 내에서 길 찾는 느낌으로 될 듯 ㅇㅇ (리얼리티 패스파이더);
    private Vector3 _maxtmp = new Vector3(10, 1, 10);
    public LayerMask la;
    private List<Vector3> _dir = new List<Vector3>();
    private List<float> _dis = new List<float>();

    public Vector3 PathFind(Transform target ,float raidius=0.5f, float speed = 10f)
    {
        if (!Physics.SphereCast(transform.position, raidius, target.position - transform.position, out RaycastHit hit, (target.position - transform.position).magnitude, la, QueryTriggerInteraction.Ignore))
        {
            _maxtmp = (target.position - transform.position).normalized;
        }
        else
        {
            _dir.Clear();
            _dis.Clear();

            for (int i = -1; i <= 1; i++)
            {
                for (int i2 = -1; i2 <= 1; i2++)
                {
                    if (((i != 0 || i2 != 0) && (i * i2 == 0)))
                        if (!(Physics.SphereCast(transform.position, raidius, Vector3.forward * i2 + Vector3.right * i, out RaycastHit hit2, 1f, la, QueryTriggerInteraction.Ignore)))//(!Physics.SphereCast(transform.position, 0.3f, Vector3.forward * i + Vector3.right * i2, out RaycastHit ray, 0.05f, la))
                        {
                            _dir.Add((Vector3.right * i + Vector3.forward * i2).normalized * 0.6f);
                        }
                }
            }
            for (int i = 0; i < _dir.Count; i++)
            {
                _dis.Add(Vector3.Distance(transform.position + _dir[i], target.position));
                if (_dir[i] == _maxtmp * -1)
                {
                    _dis[i] = 1024;
                }
            }
            _maxtmp = _dir[_dis.IndexOf(_dis.Min())];
        }
        return _maxtmp;
    }
    public Vector3 PathFind(Vector3 target, float raidius = 0.5f, float speed = 10f)
    {
        if (!Physics.SphereCast(transform.position, raidius, target - transform.position, out RaycastHit hit, (target - transform.position).magnitude, la, QueryTriggerInteraction.Ignore))
        {
            _maxtmp = (target - transform.position).normalized;
        }
        else
        {
            _dir.Clear();
            _dis.Clear();

            for (int i = -1; i <= 1; i++)
            {
                for (int i2 = -1; i2 <= 1; i2++)
                {
                    if (((i != 0 || i2 != 0) && (i * i2 == 0)))
                        if (!(Physics.SphereCast(transform.position, raidius, Vector3.forward * i2 + Vector3.right * i, out RaycastHit hit2, 1f, la, QueryTriggerInteraction.Ignore)))//(!Physics.SphereCast(transform.position, 0.3f, Vector3.forward * i + Vector3.right * i2, out RaycastHit ray, 0.05f, la))
                        {
                            _dir.Add((Vector3.right * i + Vector3.forward * i2).normalized * 0.6f);
                        }
                }
            }
            for (int i = 0; i < _dir.Count; i++)
            {
                _dis.Add(Vector3.Distance(transform.position + _dir[i], target));
                if (_dir[i] == _maxtmp * -1)
                {
                    _dis[i] = 1024;
                }
            }
            _maxtmp = _dir[_dis.IndexOf(_dis.Min())];
        }
        return _maxtmp;
    }

}