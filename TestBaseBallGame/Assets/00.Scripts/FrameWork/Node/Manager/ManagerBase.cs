// Scripts/Framework/Core/ManagerBase.cs
using UnityEngine;

namespace Bash.Framework.Core
{
    /// <summary>
    /// 모든 "매니저"들의 베이스 클래스.
    /// - GameRoot.Events 접근
    /// - Awake에서 GameRoot에 자동 등록
    /// - Initialize / OnSystemsReady / OnGameStart / OnGameEnd / Tick 라이프사이클 제공
    /// </summary>
    public abstract class ManagerBase : MonoBehaviour
    {
        /// <summary>
        /// 전역 EventHub 접근자.
        /// </summary>
        protected EventHub Events => GameRoot.Instance.Events;

        /// <summary>
        /// 다른 매니저/시스템과 무관한 초기화 로직.
        /// - Awake에서 RegisterManager 직후 한 번 호출.
        /// </summary>
        protected virtual void Initialize() { }

        /// <summary>
        /// 모든 매니저가 RegisterManager를 마치고,
        /// GameRoot.NotifySystemsReady()가 호출되었을 때 한 번 호출.
        /// - 다른 매니저에 의존하는 초기화는 여기서 하는 것을 권장.
        /// </summary>
        protected virtual void OnSystemsReady() { }

        /// <summary>
        /// 한 경기 혹은 한 게임 세션이 시작되었을 때 호출.
        /// - GameRoot.NotifyGameStart()에 의해 브로드캐스트됨.
        /// </summary>
        protected virtual void OnGameStart() { }

        /// <summary>
        /// 한 경기/세션이 종료되었을 때 호출.
        /// - GameRoot.NotifyGameEnd()에 의해 브로드캐스트됨.
        /// </summary>
        protected virtual void OnGameEnd() { }

        /// <summary>
        /// 매 프레임 호출되는 업데이트.
        /// - ManagerBase를 상속한 매니저도 Tick을 사용할 수 있다.
        /// </summary>
        protected virtual void Tick(float deltaTime) { }

        /// <summary>
        /// ManagerBase.Awake:
        /// - GameRoot에 자기 자신 등록
        /// - Initialize 호출
        /// </summary>
        protected virtual void Awake()
        {
            var root = GameRoot.Instance;
            if (root != null)
            {
                root.RegisterManager(this);
            }

            Initialize();
        }

        /// <summary>
        /// Unity Update → Tick 매핑.
        /// </summary>
        protected virtual void Update()
        {
            Tick(Time.deltaTime);
        }

        /// <summary>
        /// ManagerBase.OnDestroy:
        /// - GameRoot에서 자기 자신 제거
        /// - 자식 클래스에서 Unsubscribe 등 정리 후 base.OnDestroy() 호출 권장.
        /// </summary>
        protected virtual void OnDestroy()
        {
            var root = GameRoot.Instance;
            if (root != null)
            {
                root.UnregisterManager(this);
            }
        }

        #region Internal hooks (GameRoot에서 호출)

        /// <summary>
        /// GameRoot.NotifySystemsReady()에서 호출.
        /// </summary>
        internal void InvokeSystemsReady()
        {
            OnSystemsReady();
        }

        /// <summary>
        /// GameRoot.NotifyGameStart()에서 호출.
        /// </summary>
        internal void InvokeGameStart()
        {
            OnGameStart();
        }

        /// <summary>
        /// GameRoot.NotifyGameEnd()에서 호출.
        /// </summary>
        internal void InvokeGameEnd()
        {
            OnGameEnd();
        }

        #endregion
    }
}
