using UnityEngine;
using Bash.Framework.Core;
using Bash.Core.Unit; // RunnerPawn 참조

namespace Bash.Core.GamePlay.Environment
{
    public class BaseNode : ActorNode
    {
        public enum EBaseType { Home = 0, First = 1, Second = 2, Third = 3 }

        [Header("Identity")]
        [SerializeField] private EBaseType _baseType;
        public EBaseType BaseType => _baseType;

        [Header("Tactical Spots")]
        [SerializeField] private Transform _runnerSpot;
        [SerializeField] private Transform _defenseSpot;

        // [New] 현재 이 베이스를 점유 중인 주자
        public RunnerPawn CurrentRunner { get; private set; }
        public bool IsOccupied => CurrentRunner != null;

        protected override void Awake()
        {
            base.Awake();
            if (_runnerSpot == null) _runnerSpot = transform;
            if (_defenseSpot == null) _defenseSpot = transform;
        }

        // --- State Management ---
        public void SetOccupant(RunnerPawn runner)
        {
            CurrentRunner = runner;
        }

        public void ClearOccupant()
        {
            CurrentRunner = null;
        }

        // --- API ---
        public Vector3 GetCatchPosition() => _defenseSpot.position + Vector3.up * 1.5f;
        public Vector3 GetRunnerPosition() => _runnerSpot.position;
        public Vector3 GetTouchPosition() => transform.position;
    }
}