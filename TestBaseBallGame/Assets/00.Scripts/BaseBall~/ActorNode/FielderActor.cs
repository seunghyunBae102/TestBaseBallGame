// Scripts/Game/Baseball/Match/Fielder/FielderActor.cs
using UnityEngine;


    /// <summary>
    /// ���� ����� ����. Transform + PlayerDataSO + TeamSide ������ ��� �ִ� ���̽�.
    /// ���� �̵�/�ִϴ� ���� ���(FielderMovementModule ��)���� ���� �ȴ�.
    /// </summary>
    public class FielderActor : ActorNode
    {
        [SerializeField] private PlayerDataSO _player;
        [SerializeField] private TeamSide _teamSide;

        public PlayerDataSO Player => _player;
        public TeamSide TeamSide => _teamSide;

        public Transform CachedTransform { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            CachedTransform = transform;
        }
    }


