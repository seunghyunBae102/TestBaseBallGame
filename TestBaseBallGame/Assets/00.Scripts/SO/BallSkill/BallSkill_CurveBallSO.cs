using UnityEngine;

[CreateAssetMenu(fileName = "BallSKillSO", menuName = "SO/BaseBall/CurveBall")]
public class BallSkill_CurveBallSO : BallSKillSO
{
    public AnimationCurve curveX;
    public AnimationCurve curveY;
    //시작0 끝 0 아니면 끔찍한 결과를 보게 될 것인
    //End is 0 and Start is 0, otherwise you will see terrible results SHitTT

    public override void BallMovement(BattingBall ball)
    {
        float percent = Vector3.Distance(ball.startPos, ball.currentPos) / Vector3.Distance(ball.startPos, ball.targetPos);

        ball.ChangePos((ball.currentPos - speed * (ball.targetPos - ball.startPos).normalized * Time.deltaTime) + new Vector3(curveX.Evaluate(percent),0,curveY.Evaluate(percent)));
    }
}
