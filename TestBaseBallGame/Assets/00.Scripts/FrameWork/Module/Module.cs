// Scripts/Framework/Module/Module.cs
using UnityEngine;

namespace Bash.Framework.Core
{
    /// <summary>
    /// 게임플레이 모듈의 베이스 클래스.
    /// - OnInit / Tick / OnShutdown 3단계 라이프사이클
    /// - MonoBehaviour의 Awake / Update / OnDestroy에 매핑
    /// - ManagerBase와 달리, 시스템 레벨이 아니라 게임플레이 단위(룰, 상태, 공 모듈 등)에 사용.
    /// </summary>
    public abstract class Module : MonoBehaviour
    {
        /// <summary>
        /// 전역 EventHub 접근자.
        /// </summary>
        protected EventHub Events => GameRoot.Instance.Events;

        /// <summary>
        /// 모듈 초기화. Awake에서 한 번 호출.
        /// </summary>
        protected virtual void OnInit() { }

        /// <summary>
        /// 매 프레임 호출되는 업데이트.
        /// </summary>
        protected virtual void Tick(float deltaTime) { }

        /// <summary>
        /// 모듈 종료/정리. OnDestroy에서 한 번 호출.
        /// </summary>
        protected virtual void OnShutdown() { }

        /// <summary>
        /// Unity 라이프사이클 매핑: Awake → OnInit
        /// </summary>
        protected virtual void Awake()
        {
            OnInit();
        }

        /// <summary>
        /// Unity 라이프사이클 매핑: Update → Tick
        /// </summary>
        protected virtual void Update()
        {
            Tick(Time.deltaTime);
        }

        /// <summary>
        /// Unity 라이프사이클 매핑: OnDestroy → OnShutdown
        /// </summary>
        protected virtual void OnDestroy()
        {
            OnShutdown();
        }
    }
}
