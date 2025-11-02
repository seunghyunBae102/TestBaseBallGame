using UnityEngine;

public class Batting : Actor
{

    public void ThrowBall(Vector3 start, Vector3 targetPos, BallSKillSO ballSkill)
    {
        GetOrAddCompo<BattingBall>().InitBall(start, targetPos, ballSkill);
    }

    public void SwingBat()
    {
        
    }
}
