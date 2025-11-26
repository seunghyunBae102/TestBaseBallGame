// Scripts/Framework/Core/GameRoot.cs
using System.Collections.Generic;
using UnityEngine;

namespace Bash.Framework.Core
{
    /// <summary>
    /// 게임 전체의 루트 오브젝트.
    /// - 전역 EventHub 제공
    /// - Manager 레지스트리 제공
    /// - 필요 시 시스템 준비/게임 시작/끝을 브로드캐스트
    /// </summary>
    public class GameRoot : MonoBehaviour
    {
        private static GameRoot _instance;

        public static GameRoot Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameRoot>();
                    if (_instance == null)
                    {
                        Debug.LogError("[GameRoot] 씬에 GameRoot가 없습니다.");
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// 전역 이벤트 허브.
        /// </summary>
        public EventHub Events { get; private set; } = new EventHub();

        /// <summary>
        /// 등록된 모든 매니저.
        /// </summary>
        private readonly List<ManagerBase> _managers = new();

        private bool _systemsReadyFired;
        private bool _gameStarted;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                // 중복 GameRoot 방지
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                Events?.Clear();
                _managers.Clear();
                _instance = null;
            }
        }

        #region Manager Registry

        /// <summary>
        /// ManagerBase가 Awake에서 자동으로 자신을 등록할 때 사용.
        /// </summary>
        internal void RegisterManager(ManagerBase manager)
        {
            if (manager == null) return;
            if (_managers.Contains(manager)) return;

            _managers.Add(manager);
        }

        /// <summary>
        /// ManagerBase가 OnDestroy에서 자동으로 자신을 해제할 때 사용.
        /// </summary>
        internal void UnregisterManager(ManagerBase manager)
        {
            if (manager == null) return;
            _managers.Remove(manager);
        }

        /// <summary>
        /// 등록된 매니저 중 타입 T(클래스 또는 인터페이스)인 것을 반환.
        /// 없으면 null.
        /// </summary>
        public T GetManager<T>() where T : class
        {
            for (int i = 0; i < _managers.Count; i++)
            {
                if (_managers[i] is T t)
                    return t;
            }
            return null;
        }

        /// <summary>
        /// 모든 매니저에 대해 OnSystemsReady를 한 번 호출.
        /// - 보통 상위 GameManager 혹은 씬 초기화 코드에서
        ///   모든 매니저가 생성된 후 한 번만 호출해주면 된다.
        /// </summary>
        public void NotifySystemsReady()
        {
            if (_systemsReadyFired) return;
            _systemsReadyFired = true;

            foreach (var m in _managers)
            {
                m.InvokeSystemsReady();
            }
        }

        /// <summary>
        /// 모든 매니저에 대해 OnGameStart 호출.
        /// - 야구 한 경기 시작 시점 등에서 사용.
        /// </summary>
        public void NotifyGameStart()
        {
            if (_gameStarted) return;
            _gameStarted = true;

            foreach (var m in _managers)
            {
                m.InvokeGameStart();
            }
        }

        /// <summary>
        /// 모든 매니저에 대해 OnGameEnd 호출.
        /// - 야구 한 경기 끝났을 때 등에서 사용.
        /// </summary>
        public void NotifyGameEnd()
        {
            if (!_gameStarted) return;
            _gameStarted = false;

            foreach (var m in _managers)
            {
                m.InvokeGameEnd();
            }
        }

        #endregion
    }
}
