// Scripts/Framework/Node/ActorManager.cs
using System.Collections.Generic;
using UnityEngine;
using Bash.Framework.Core;

namespace Bash.Framework.Node
{
    /// <summary>
    /// 씬 내의 모든 ActorNode를 관리하는 매니저.
    /// - ActorNode.OnEnable/OnDisable에서 자동 등록/해제
    /// - 타입별 조회, 가장 가까운 Actor 조회 등의 헬퍼 제공
    /// - 팀/포지션 등 게임 도메인 정보는 파생 Actor나 별도 컴포넌트에서 처리.
    /// </summary>
    public class ActorManager : ManagerBase, IActorManager
    {
        private readonly List<ActorNode> _actors = new();

        /// <summary>
        /// 현재 등록된 모든 액터에 대한 ReadOnly 액세스.
        /// </summary>
        public IReadOnlyList<ActorNode> Actors => _actors;

        /// <summary>
        /// ActorNode에서 OnEnable 시 호출.
        /// </summary>
        public void RegisterActor(ActorNode actor)
        {
            if (actor == null) return;
            if (_actors.Contains(actor)) return;

            _actors.Add(actor);
        }

        /// <summary>
        /// ActorNode에서 OnDisable 시 호출.
        /// </summary>
        public void UnregisterActor(ActorNode actor)
        {
            if (actor == null) return;
            _actors.Remove(actor);
        }

        /// <summary>
        /// 특정 타입 T인 액터들을 모두 가져오기.
        /// </summary>
        public List<T> GetActorsOfType<T>() where T : ActorNode
        {
            var result = new List<T>();
            for (int i = 0; i < _actors.Count; i++)
            {
                if (_actors[i] is T t)
                    result.Add(t);
            }
            return result;
        }

        /// <summary>
        /// 월드 좌표 기준 가장 가까운 T 타입 액터를 찾는다.
        /// - maxDistance가 0 이하이면 거리 제한 없음.
        /// </summary>
        public bool TryGetClosestActor<T>(
            Vector3 fromPosition,
            out T result,
            float maxDistance = 0f)
            where T : ActorNode
        {
            result = null;
            float bestSqr = float.MaxValue;

            for (int i = 0; i < _actors.Count; i++)
            {
                if (_actors[i] is not T candidate) continue;

                float sqr = (candidate.transform.position - fromPosition).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    if (maxDistance > 0f && sqr > maxDistance * maxDistance)
                        continue;

                    bestSqr = sqr;
                    result = candidate;
                }
            }

            return result != null;
        }

        /// <summary>
        /// 씬이 리셋될 때 등, 내부 리스트를 강제로 비우고 싶을 때 사용할 수 있다.
        /// (보통은 필요 없음)
        /// </summary>
        public void ClearActors()
        {
            _actors.Clear();
        }
    }
}
