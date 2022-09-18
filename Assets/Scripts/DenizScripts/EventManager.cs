using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Managers {

    // EventManager contains central events which are listened by multiple other scripts.

    public class EventManager : MonoBehaviour
    {
        #region Singleton

        public static EventManager Instance;

        private void Awake()
        {   
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        #endregion

        #region Events

        public UnityAction OnGameStart;
        public UnityAction OnGamePause;
        public UnityAction OnGameReset;
        public UnityAction OnGameEnd;
        public UnityAction<int> OnWaveStart;
        public UnityAction OnWaveEnd;
        public UnityAction<float> OnUpdateTimer;
        public UnityAction OnWaveFinish;

        public UnityAction OnNextLevel;
        public UnityAction OnRestartLevel;
        public UnityAction<int> OnLoadLevel;


        #endregion
    }
}

