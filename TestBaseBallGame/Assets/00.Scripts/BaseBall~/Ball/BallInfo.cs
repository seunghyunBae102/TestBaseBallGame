using UnityEngine;

/// <summary>
/// 공의 런타임 상태.
/// </summary>
[System.Serializable]
public struct BallState
{
 public Vector3 position;
 public Vector3 velocity;
 public bool isAlive;
}

/// <summary>
/// 공 시뮬레이션에 필요한 환경 정보.
/// </summary>
public struct BallContext
{
 public Vector3 targetPos;
 public float gravity;
}

/// <summary>
/// 타구 궤적의 한 지점.
/// </summary>
[System.Serializable]
public struct BallPathPoint
{
 public float time; // 이 시점까지 경과 시간
 public Vector3 position; // 공 위치
 public Vector3 velocity; // 공 속도

 public BallPathPoint(float time, Vector3 position, Vector3 velocity)
 {
 this.time = time;
 this.position = position;
 this.velocity = velocity;
 }
}

public interface IBallBehaviour
{
 void Initialize(ref BallState state, in BallContext ctx);
 void SimulateStep(ref BallState state, in BallContext ctx, float dt);
}

public interface IBallStepModifier
{
 void Modify(ref BallState state, in BallContext ctx, float dt);
}
