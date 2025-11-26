// Scripts/Framework/Node/ActorNode.cs
using UnityEngine;
using Bash.Framework.Core;

namespace Bash.Framework.Node
{
    /// <summary>
    /// 필드 위의 액터(러너, 필더, 투수, 타자 등)를 위한 베이스 클래스.
    /// - BashNode(부모/자식 트리) 상속
    /// - IActorManager에 의해 관리됨
    /// - 팀/역할/스탯 등 도메인 정보는 파생 클래스에서 구현
    /// </summary>
    public abstract class ActorNode : BashNode
    {
        IActorManager ActorManager =>
            GameRoot.Instance.GetManager<IActorManager>();
        protected virtual void OnEnable()
        {
            //base.OnEnable();

            var root = GameRoot.Instance;
            var actorManager = root != null ? root.GetManager<IActorManager>() : null;
            actorManager?.RegisterActor(this);
        }

        protected virtual void OnDisable()
        {
            //base.OnDisable();

            var root = GameRoot.Instance;
            var actorManager = root != null ? root.GetManager<IActorManager>() : null;
            actorManager?.UnregisterActor(this);
        }
    }
}
