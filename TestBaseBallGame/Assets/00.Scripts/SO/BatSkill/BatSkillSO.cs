using System;
using UnityEngine;

[Serializable]
public struct BatExtenceData
{
    public Vector3 Pos;
    public Vector3 Scale;

    public BatExtenceData(Vector3 pos, Vector3 scale)
    {
        Pos = pos;
        Scale = scale;
    }
}

[CreateAssetMenu(fileName = "BatSkillSO", menuName = "SO/BaseBallSkill/BatSkill/BatSkillSO")]
public class BatSkillSO : ScriptableObject
{
    public Vector3 SweetSpotPos; //�������� - �� ������ ���ε�, �� �ܿ��� �� �Ѷ����ų� �׷� ���� �ֳ� https://blog.naver.com/ysuny2/223159174301

    public BatExtenceData BatHitBox;

    public virtual bool HitBat(BattingBall ball,Batter bat)
    {
        Vector3 leftDown = BatHitBox.Pos + bat.BatPosition - (BatHitBox.Scale / 2);
        Vector3 rightUp = BatHitBox.Pos + bat.BatPosition + (BatHitBox.Scale / 2);
        if (ball.currentPos.x >= leftDown.x && ball.currentPos.x <= rightUp.x &&
            ball.currentPos.y >= leftDown.y && ball.currentPos.y <= rightUp.y &&
            ball.currentPos.z >= leftDown.z && ball.currentPos.z <= rightUp.z)
        {
            BallHit(ball, bat);
            return true;
        }

        return false;
    }

    public virtual void BallHit(BattingBall ball, Batter bat)
    {
        // upper y -> high fly
        // middle y -> normal hit
        // lower y -> ground hit

        // closer SweetSpotPos -> more power

    }

}
