using UnityEngine;

[CreateAssetMenu(fileName = "BallSKillSO", menuName = "SO/BaseBall/BallSKillSO")]
public class BallSKillSO : ScriptableObject
{
    public float speed =10;

    //public virtual void BallMovement(ref Vector3 pos)
    //{
    //    pos -= speed * Vector3.forward * Time.deltaTime;
    //}
    public virtual void BallMovement(BattingBall ball)
    {
        ball.ChangePos(ball.currentPos - speed * (ball.targetPos - ball.startPos).normalized * Time.deltaTime);
    }

    public virtual void BallStop(BattingBall ball)
    {
        
    }
}
