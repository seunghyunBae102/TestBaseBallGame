using UnityEngine;



public class Batter : ActorComponent
{
    //Ÿ�� ��Ʈ ��ġ ,ĳ����,���� �ִϸ��̼� ��� �߰� �ʿ�

    public Vector3 BatPosition;
    public BatSkillSO BatSO;

    public bool CheckBallHit(BattingBall ball)
    {

        return BatSO.HitBat(ball,this);

        //return false;
        ////Ÿ�� ����
        //float distance = Vector3.Distance(ball.currentPos, BatPosition);
        //if (distance < BatSO.HitDistance)
        //{
        //    //Ÿ�� ����
        //    Debug.Log("Hit the Ball!");
        //    //Ÿ�� ó�� ���� �߰� �ʿ�
        //    ball.OnStopBall?.Invoke(ball);
        //}
        //return false;
    }

    public void SwingBat()
    {

        //GameManager.Instance.GetCompo<BaseBallGameManager>().GetCompo<BaseBallBattingManager>().BatterSwingBat(this);
    }
}
