
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseBallRunningManager : Actor, IGetCompoable<BaseBallGameManager>
{
    [HideInInspector]
    public BaseBallGameManager Mom;

    public List<Vector3> ballPos { get; private set; } = new List<Vector3>();

    public UnityEvent OnBallPosChanged;

    private void Start()
    {
        if (Mom == null)
        {
            //Transform trm = GetComponent<Transform>();

            //while(trm.parent != null && trm.gameObject.GetComponent<GameManager>())
            //{
            //    trm = trm.parent;
            //}
            //Init(trm.GetComponent<GameManager>());
            if (GameManager.Instance != null)
                if (GameManager.Instance.GetCompo<BaseBallGameManager>() != null)
                    Init(GameManager.Instance.GetCompo<BaseBallGameManager>());

        }
    }
    public virtual void Init(BaseBallGameManager mom)
    {
        mom.AddCompoDic(this.GetType(), this);
        Mom = mom;
    }

    public void ChangeBallPos(List<Vector3> ballpos)
    {
        ballPos = ballpos;
        OnBallPosChanged?.Invoke();
    }

    public Vector3 GetBallPos(int time)
    {
        if(ballPos == null)
            return Vector3.zero;
        if(ballPos.Count <= 0)
            return Vector3.zero;

        return ballPos[Mathf.Clamp(time, 0, ballPos.Count - 1)];
    }

    public void ClearBallPos()
    {
        ballPos.Clear();
        //OnBallPosChanged?.Invoke();
    }

    public void RemoveBallPosPerTime()
    {
        if (ballPos.Count > 0)
        {
            ballPos.RemoveAt(0);
            OnBallPosChanged?.Invoke();
        }
    }

    public void RegisterEvents(BaseBallGameManager main)
    {
        throw new System.NotImplementedException();
    }

    public void UnregisterEvents(BaseBallGameManager main)
    {
        throw new System.NotImplementedException();
    }


}