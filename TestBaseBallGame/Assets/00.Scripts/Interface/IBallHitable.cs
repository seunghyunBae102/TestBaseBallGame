using UnityEngine;

public interface IBallHitable
{
    void OnBallHit(Vector3 hitPoint, Vector3 hitNormal);
}
