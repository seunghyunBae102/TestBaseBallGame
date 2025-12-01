// 테스트용 (WorldRoot에 임시로 붙여서 사용)
using UnityEngine;
using Bash.Core.Ball;
using Bash.Framework.Core;

public class BallTester : ActorNode
{
    public GameObject BallPrefab; // BallNode + BallPhysicsCompo가 달린 프리팹

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var go = Instantiate(BallPrefab);
            var ball = go.GetComponent<BallNode>();

            // 가상 계층 연결
            ball.AttachTo(this);

            // 발사! (앞으로, 약간 위로)
            ball.Launch(transform.forward + transform.up * 0.5f, 30.0f);
        }
    }
}