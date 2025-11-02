using System;
using UnityEngine;
using UnityEngine.Events;



public class Batter : ActorComponent
{
    //타구 배트 위치 ,캐릭터,스윙 애니메이션 등등 추가 필요

    public Vector3 BatPosition;
    public BatSkillSO BatSO;

    public event Action<Vector2> OnMoveBatPosition;

    protected bool _isSwinging = false;

    public void MoveBat(Vector2 pos)
    {
        BatPosition = new Vector3(pos.x, pos.y, 0f);
        OnMoveBatPosition?.Invoke(pos);
    }

    public bool CheckBallHit(BattingBall ball)
    {
        if(!_isSwinging)
            return false;

        return BatSO.HitBat(ball,this);

        //return false;
        ////타구 판정
        //float distance = Vector3.Distance(ball.currentPos, BatPosition);
        //if (distance < BatSO.HitDistance)
        //{
        //    //타구 성공
        //    Debug.Log("Hit the Ball!");
        //    //타구 처리 로직 추가 필요
        //    ball.OnStopBall?.Invoke(ball);
        //}
        //return false;
    }

    public void SwingBat()
    {
        _isSwinging = true;
        
        GameManager.Instance.GetCompo<TimerManager>().AddTimer(EndSwing,BatSO.SwingDuration);
        //GameManager.Instance.GetCompo<BaseBallGameManager>().GetCompo<BaseBallBattingManager>().BatterSwingBat(this);
    }

    public void EndSwing()
    {
        _isSwinging = false;
    }
}
