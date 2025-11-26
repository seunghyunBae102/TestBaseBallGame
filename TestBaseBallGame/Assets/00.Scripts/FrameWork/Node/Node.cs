// Scripts/Framework/Node/BashNode.cs
using System.Collections.Generic;
using UnityEngine;
using Bash.Framework.Core;

namespace Bash.Framework.Node
{
    /// <summary>
    /// Unity의 GameObject + Transform 개념을 얇게 감싼 "논리 노드".
    /// - 부모/자식 관계를 BashNode 단위로 관리
    /// - 자식/부모 검색 헬퍼 제공
    /// - 실제 GameObject는 그대로 사용하고, BashNode가 "게임 논리의 기준"이 됨
    /// </summary>
    public class BashNode : MonoBehaviour
    {
        /// <summary>이 노드의 부모 노드 (없으면 null).</summary>
        public BashNode Parent { get; private set; }

        private readonly List<BashNode> _children = new();
        /// <summary>이 노드의 자식 노드들.</summary>
        public IReadOnlyList<BashNode> Children => _children;

        /// <summary>
        /// 전역 EventHub 접근자 (필요하면 사용).
        /// </summary>
        protected EventHub Events => GameRoot.Instance.Events;

        #region Unity lifecycle

        protected virtual void Awake()
        {
            // 초기 Parent/Children 세팅
            RefreshParent();
            RefreshChildren();
        }

        protected virtual void OnTransformParentChanged()
        {
            RefreshParent();
        }

        protected virtual void OnTransformChildrenChanged()
        {
            RefreshChildren();
        }

        #endregion

        #region Parent / Children 관리

        private void RefreshParent()
        {
            var parentTransform = transform.parent;
            BashNode newParent = null;

            if (parentTransform != null)
                newParent = parentTransform.GetComponentInParent<BashNode>();

            if (Parent == newParent) return;

            // 이전 부모에서 제거
            if (Parent != null)
                Parent._children.Remove(this);

            Parent = newParent;

            // 새 부모에 등록
            if (Parent != null && !Parent._children.Contains(this))
                Parent._children.Add(this);
        }

        private void RefreshChildren()
        {
            _children.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                var childTr = transform.GetChild(i);
                var childNode = childTr.GetComponent<BashNode>();
                if (childNode != null && !_children.Contains(childNode))
                {
                    _children.Add(childNode);
                    childNode.Parent = this;
                }
            }
        }

        #endregion

        #region BashNode / Component 검색 헬퍼

        /// <summary>
        /// 자식 노드 중 T 타입을 가진 첫 번째 노드를 반환 (자식 트리 전체 검색).
        /// </summary>
        public T FindInChildren<T>() where T : BashNode
        {
            foreach (var child in _children)
            {
                if (child is T tNode)
                    return tNode;

                var deeper = child.FindInChildren<T>();
                if (deeper != null)
                    return deeper;
            }

            return null;
        }

        /// <summary>
        /// 부모 방향으로 올라가며 T 타입 노드를 찾는다.
        /// </summary>
        public T FindInParents<T>() where T : BashNode
        {
            var current = Parent;
            while (current != null)
            {
                if (current is T tNode)
                    return tNode;

                current = current.Parent;
            }
            return null;
        }

        /// <summary>
        /// 이 노드 또는 자식 노드들에서 특정 Component를 찾는다.
        /// (Transform 기준이 아니라 BashNode 트리 기준으로 찾고 싶을 때 사용)
        /// </summary>
        public T GetComponentInNodeTree<T>() where T : Component
        {
            var comp = GetComponent<T>();
            if (comp != null)
                return comp;

            // 자식 노드 기준으로 검색
            foreach (var child in _children)
            {
                comp = child.GetComponentInNodeTree<T>();
                if (comp != null)
                    return comp;
            }

            return null;
        }

        #endregion
    }
}
