using UnityEngine;

public interface IBallHitable
{
    void OnHitByBall(HitedBall ball, RaycastHit hit);
}
