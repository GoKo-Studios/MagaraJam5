using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Assets.Scripts.Managers {
    public class GUIManager : MonoBehaviour
    {
        #region Singleton

        public static GUIManager Instance;

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

        [SerializeField] private TextMeshProUGUI _waveText;
        [SerializeField] private GameObject _timerObject;
        [SerializeField] private TextMeshProUGUI _timerText;

        private void OnEnable() {
            EventManager.Instance.OnWaveStart += OnWaveStart;
            EventManager.Instance.OnUpdateTimer += OnUpdateTimer;
            EventManager.Instance.OnWaveEnd += OnWaveEnd;
        }

        private void OnDisable() {
            EventManager.Instance.OnWaveStart -= OnWaveStart;
            EventManager.Instance.OnUpdateTimer -= OnUpdateTimer;
            EventManager.Instance.OnWaveEnd -= OnWaveEnd;
        }

        public void Button_SpawnAggressiveMob() {
            MobSpawnerManager.Instance.SpawnMobWithPooling(ResourceLoader.LoadResource<ScriptableObject>("Objects/PoolableObjects/New Spider") as MobDataBase);
        }

        public void Button_SpawnPassiveMob() {
            MobSpawnerManager.Instance.SpawnMobWithPooling(ResourceLoader.LoadResource<ScriptableObject>("Objects/PoolableObjects/NewPassiveMob") as MobDataBase);
        }

        private void UpdateWaveTimer(int time) {
            
        }

        public void Button_GameStart() {
            EventManager.Instance.OnGameStart?.Invoke();
        }

        private void OnWaveEnd() {
            //_waveText.enabled = false;
            //_timerObject.SetActive(true);
        }

        private void OnUpdateTimer(float timer) {
            _timerText.text = ((int)timer).ToString();
        }

        private void OnWaveStart(int waveRemeaning) {
            _waveText.enabled = true;
            _timerObject.SetActive(true);
            _waveText.text = "Wave Remaning " + waveRemeaning;
        }
    }
}

