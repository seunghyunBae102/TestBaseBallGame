using UnityEngine;
using Bash.Framework.Core;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private GameObject _ballPrefab;

    //private void Start()
    //{
    //    // 1. 시스템 초기화
    //    var root = GameRoot.Instance;
    //    var pool = root.SystemRoot.AddCompo<PoolManagerCompo>();
    //    var sound = root.SystemRoot.AddCompo<SoundManagerCompo>();

    //    pool.RegisterPrefab("Ball", _ballPrefab);

    //    // 2. 게임 월드 구성 (가상 계층 생성)
    //    var fieldNode = new ActorNode("Field", root.WorldRoot);

    //    // 투수 노드 생성
    //    var pitcherNode = new ActorNode("Pitcher", fieldNode);
    //    pitcherNode.AddCompo<PitchingModule>(); // 투구 로직 부착

    //    // 3. 가상 세계 시작 로그
    //    Debug.Log("Virtual Framework Initialized.");
    //}
}