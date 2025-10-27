using UnityEngine;
[CreateAssetMenu(fileName = "FieldBallSKillSO", menuName = "SO/FieldBaseBall/FieldBallSKillSO")]
public class FieldBallSKilSO : ScriptableObject,IHitedBallSKillable
{
    public virtual void OnMove()
    {

    }

    public virtual void OnCollide(HitedBall ballm, RaycastHit hit)
    {

    }

    public virtual void OnHit(HitedBall ballm, RaycastHit hit)
    {

    }

}
