// Scripts/Framework/Node/IActorManager.cs
using System.Collections.Generic;
using UnityEngine;

namespace Bash.Framework.Node
{
    /// <summary>
    /// 액터 매니저 인터페이스.
    /// - ActorNode 등록/해제
    /// - 타입별 조회, 가장 가까운 액터 조회 등의 기능 정의
    /// </summary>
    public interface IActorManager
    {
        IReadOnlyList<ActorNode> Actors { get; }

        void RegisterActor(ActorNode actor);
        void UnregisterActor(ActorNode actor);

        List<T> GetActorsOfType<T>() where T : ActorNode;

        bool TryGetClosestActor<T>(
            Vector3 fromPosition,
            out T result,
            float maxDistance = 0f)
            where T : ActorNode;

        void ClearActors();
    }
}
