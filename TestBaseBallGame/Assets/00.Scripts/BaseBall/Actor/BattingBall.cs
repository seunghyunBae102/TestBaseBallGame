using System;
using UnityEngine;

public class BattingBall : ActorComponent
{
    public Vector3 currentPos { get; protected set; }
    public Vector3 startPos { get; protected set; }
    public Vector3 targetPos { get; protected set; }
    //protected Vector3 _currentPos;
    //protected Vector3 _startPos;
    //protected Vector3 _targetPos;

    //public Vector3 currentPos { get { return _currentPos; } protected set; } 
    //public Vector3 startPos;
    //public Vector3 targetPos;

    protected bool _isPitching = false;

    public Action<Vector3> OnMoving;
    public Action<BattingBall> OnStopBall;

    protected BallSKillSO _ballSkill;

    public void InitBall(Vector3 start,Vector3 targetPos, BallSKillSO ballSkill)
    {
        startPos = start;
        currentPos = start;
        this.targetPos = targetPos;
        _ballSkill = ballSkill;
        _isPitching = true;
    }

    protected void Update()
    {
        if (_isPitching)
        {
            _ballSkill.BallMovement(this);


            //if(currentPos.z < 0f)
            //{
            //    StopBall();
            //}
        }
    }

    public void ChangePos(Vector3 pos)
    {
        if(CheckBallHit(pos))
        {
            StopBall();
            return;
        }

        currentPos = pos;

        OnMoving?.Invoke(currentPos);
    }

    public bool CheckBallHit(Vector3 pos)
    {
        Debug.Log("FixHere");
        return false;//Mom.GetCompo<Batter>().CheckBallHit(this);
    }

    public void AddPos(Vector3 pos)
    {
        ChangePos(currentPos + pos);
    }

    protected void StopBall()
    {
        _isPitching = false;
        _ballSkill.BallStop(this);
        OnStopBall?.Invoke(this);
    }
}
