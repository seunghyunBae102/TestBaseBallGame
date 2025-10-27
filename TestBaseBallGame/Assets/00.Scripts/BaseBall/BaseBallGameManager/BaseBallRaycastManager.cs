using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct RaycastComboSet
{
    public RaycastCommand raycastCommand;
    public Action<RaycastHit,float> action;
    public RaycastComboSet(RaycastCommand rc, Action<RaycastHit, float> a)
    {
        raycastCommand = rc;
        action = a;
    }
}
public struct SpherecastComboSet
{
    public SpherecastCommand raycastCommand;
    public Action<RaycastHit, float> action;
    public SpherecastComboSet(SpherecastCommand rc, Action<RaycastHit, float> a)
    {
        raycastCommand = rc;
        action = a;
    }
}

public class BaseBallRaycastManager : BaseBallGetableManager
{
    protected List<RaycastComboSet> _raycastScheduls = new();
    protected List<SpherecastComboSet> _spherecastScheduls = new();

    public void AddRaycastSchedul(RaycastComboSet a)
    {
        _raycastScheduls.Add(a);
    }
    public void AddSpherecastSchedul(SpherecastComboSet a)
    {
        _spherecastScheduls.Add(a);
    }
    private void FixedUpdate()
    {
        if (_raycastScheduls.Count > 0)
        {
            NativeArray<RaycastCommand> raycastCom = new NativeArray<RaycastCommand>(_raycastScheduls.Count, Allocator.TempJob);
            NativeArray<RaycastHit> raycastHit = new NativeArray<RaycastHit>(_raycastScheduls.Count, Allocator.TempJob);

            for (int i = 0; i < _raycastScheduls.Count; i++)
            {
                raycastCom[i] = _raycastScheduls[i].raycastCommand;
                raycastHit[i] = new RaycastHit();
            }

            var jobbs = RaycastCommand.ScheduleBatch(raycastCom, raycastHit, _raycastScheduls.Count);

            jobbs.Complete();

            {
                for (int i = 0; i < _raycastScheduls.Count; i++)
                {
                  _raycastScheduls[i].action?.Invoke(raycastHit[i],Time.fixedDeltaTime);
                }

                raycastHit.Dispose();
                raycastCom.Dispose();
            }

            _raycastScheduls.Clear();
        }

        if (_spherecastScheduls.Count > 0)
        {
            NativeArray<SpherecastCommand> raycastCom = new NativeArray<SpherecastCommand>(_spherecastScheduls.Count, Allocator.TempJob);
            NativeArray<RaycastHit> raycastHit = new NativeArray<RaycastHit>(_spherecastScheduls.Count, Allocator.TempJob);

            for (int i = 0; i < _spherecastScheduls.Count; i++)
            {
                raycastCom[i] = _spherecastScheduls[i].raycastCommand;
                raycastHit[i] = new RaycastHit();
            }

            var jobbs = SpherecastCommand.ScheduleBatch(raycastCom, raycastHit, _spherecastScheduls.Count);

            jobbs.Complete();

            {
                for (int i = 0; i < _spherecastScheduls.Count; i++)
                {
                    _spherecastScheduls[i].action?.Invoke(raycastHit[i], Time.fixedDeltaTime);
                }

                raycastHit.Dispose();
                raycastCom.Dispose();
            }

            _spherecastScheduls.Clear();
        }

    }
}

