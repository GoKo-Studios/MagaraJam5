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

        [SerializeField] private GameObject _waveObject;
        [SerializeField] private TextMeshProUGUI _waveText;
        [SerializeField] private TextMeshProUGUI _timerText;

        private void Start() {
            EventManager.Instance.OnWaveStart += OnWaveStart;
            EventManager.Instance.OnUpdateTimer += OnUpdateTimer;
            EventManager.Instance.OnWaveEnd += OnWaveEnd;
            EventManager.Instance.OnWaveFinish += OnWaveFinish;
        }

        private void OnDisable() {
            EventManager.Instance.OnWaveStart -= OnWaveStart;
            EventManager.Instance.OnUpdateTimer -= OnUpdateTimer;
            EventManager.Instance.OnWaveEnd -= OnWaveEnd;
            EventManager.Instance.OnWaveFinish -= OnWaveFinish;
        }

        public void Button_SpawnAggressiveMob() {
            MobSpawnerManager.Instance.SpawnMobWithPooling(ResourceLoader.LoadResource<ScriptableObject>("Objects/PoolableObjects/New Spider") as MobDataBase);
        }

        public void Button_SpawnPassiveMob() {
            MobSpawnerManager.Instance.SpawnMobWithPooling(ResourceLoader.LoadResource<ScriptableObject>("Objects/PoolableObjects/NewPassiveMob") as MobDataBase);
        }

        public void Button_GameStart() {
            EventManager.Instance.OnGameStart?.Invoke();
            _waveObject.SetActive(true);
        }

        private void OnWaveEnd() {

        }

        private void OnUpdateTimer(float timer) {
            _timerText.text = "Time Until Next Wave: " + ((int)timer).ToString();
        }

        private void OnWaveStart(int waveRemaining) {
            _waveObject.SetActive(true);
            _waveText.text = "Wave Remaining " + waveRemaining;
        }

        private void OnWaveFinish() {
            _waveObject.SetActive(false);
        }
    }
}

