using UnityEngine;

[CreateAssetMenu(fileName = "BallSKillSO", menuName = "SO/BaseBall/CurveBall")]
public class BallSkill_CurveBallSO : BallSKillSO
{
    public AnimationCurve curveX;
    public AnimationCurve curveY;
    //����0 �� 0 �ƴϸ� ������ ����� ���� �� ����
    //End is 0 and Start is 0, otherwise you will see terrible results SHitTT

    public override void BallMovement(BattingBall ball)
    {
        float percent = Vector3.Distance(ball.startPos, ball.currentPos) / Vector3.Distance(ball.startPos, ball.targetPos);

        ball.ChangePos((ball.currentPos - speed * (ball.targetPos - ball.startPos).normalized * Time.deltaTime) + new Vector3(curveX.Evaluate(percent),0,curveY.Evaluate(percent)));
    }
}
