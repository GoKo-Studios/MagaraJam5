using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Enums;
using System.Collections;

namespace Assets.Scripts.Managers {
    public class WaveManager : MonoBehaviour
    {
        #region Singleton

        public static WaveManager Instance;

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

        [SerializeField] private List<MobDataBase> _mobAggressiveList; 
        [SerializeField] private List <MobDataBase> _mobPassiveList;
        [SerializeField] private GameObject _passiveMobLocations;
        [SerializeField] private int _waveCount;
        [SerializeField] private float _waveDuration;
        [SerializeField] private float _spawnInterval;
        [SerializeField] private float _levelFinishTime;

        [SerializeField] private int _currentWave;
        [SerializeField] private float _baseWaveValue;
        [SerializeField] private int _timeBetweenWaves;

        [SerializeField] private float _startTime;

        private List<MobDataBase> _generatedWave = new List<MobDataBase>();
        [SerializeField] public List<GameObject> _spawnedEnemies;
        private float _waveValue;
        private float _waveTimer;
        private float _spawnTimer;
        private float _betweenTimer;
        private float _currentRemaningTime;
        [SerializeField] private WaveManagerStates _state;
        [SerializeField] private bool _isEnabled = false;

        private void OnEnable() {
            EventManager.Instance.OnGameStart += OnGameStart;
            EventManager.Instance.OnGamePause += OnGamePause;
        }

        private void OnDisable() {
            EventManager.Instance.OnGameStart -= OnGameStart;
            EventManager.Instance.OnGamePause -= OnGamePause;
        }

        private void OnGameStart() {
            StartManager();
            ProocedToNextWave();
        }

        private void OnGamePause() {

        }

        private IEnumerator StartTime() {
            yield return new WaitForSecondsRealtime(_startTime);
            EventManager.Instance.OnGameStart?.Invoke();
        }

        private void Start()
        {  
            _currentRemaningTime = _levelFinishTime;
            PopulateDataLists();
            SpawnPassiveMobs();   

            StartCoroutine(StartTime());
             
        }

         private void PopulateDataLists() {
            _mobPassiveList.Clear();
            _mobPassiveList.AddRange(ResourceLoader.LoadResources<MobDataBase>("Objects/PoolableObjects/Passive"));

            _mobAggressiveList.Clear();
            _mobAggressiveList.AddRange(ResourceLoader.LoadResources<MobDataBase>("Objects/PoolableObjects/Aggressive"));
        }

        void Update()
        {
            if (!_isEnabled) return;

            if (_currentRemaningTime <= 0f) {
                StopManager();
            }
            else {
                _currentRemaningTime -= Time.deltaTime;
            }

            switch (_state) {
                case WaveManagerStates.InWave:
                    if (_spawnTimer <= 0) {
                        if (_generatedWave.Count > 0) {
                            GameObject obj = MobSpawnerManager.Instance.SpawnMobWithPooling(_generatedWave[0]);
                            _generatedWave.RemoveAt(0);
                            _spawnTimer = _spawnInterval;
                            _spawnedEnemies.Add(obj);
                        }  
                    }
                    else {
                        _spawnTimer -= Time.deltaTime;
                    }

                    // When wave timer runs out.
                    if (_waveTimer > 0) {
                        _waveTimer -= Time.deltaTime;
                        EventManager.Instance.OnUpdateTimer?.Invoke(_waveTimer);
                    }
                    else {
                        EndWave();
                    }

                    // There are no remaining spawned enemies.
                    if (_spawnedEnemies.Count <= 0) {
                        EndWave();
                    }
                    
                break;

                case WaveManagerStates.BetweenWave:
                    if (_betweenTimer <= 0) {
                        ProocedToNextWave();
                    }
                    else {
                        _betweenTimer -= Time.deltaTime;
                        EventManager.Instance.OnUpdateTimer?.Invoke((int)_betweenTimer);
                    }
                break;
            }
        }

        private void SpawnPassiveMobs() {
            foreach (Transform location in _passiveMobLocations.GetComponentInChildren<Transform>()) {
                MobDataBase data = _mobPassiveList[Random.Range(0, _mobPassiveList.Count)];
                GameObject obj = MobSpawnerManager.Instance.SpawnMobWithPoolingAndLocation(data, location);
            }
        }

        private void EndWave() {
            _waveTimer = _waveDuration;
            EventManager.Instance.OnWaveEnd?.Invoke();
            _state = WaveManagerStates.BetweenWave;
        }

        public void ProocedToNextWave() {
            _state = WaveManagerStates.Idle;
            _betweenTimer = _timeBetweenWaves;

            if (_currentWave < _waveCount) {
                _currentWave++;
                GenerateWave();
            }
            else {
                // Stop Wave Manager
                StopManager();
                EventManager.Instance.OnWaveFinish?.Invoke();
            }

        }

        public void StopManager() {
            _isEnabled = false;
        }

        public void StartManager() {
            _isEnabled = true;
        }

        private void GenerateWave() {
                           
            _waveValue = _baseWaveValue + _currentWave * 10.0f;
            GenerateMobs();

            _waveTimer = _waveDuration;
        }

        private void GenerateMobs() {
            List<MobDataBase> generatedEnemies = new List<MobDataBase>();
            while (_waveValue > 0) {
                int randomEnemyID = Random.Range(0, _mobAggressiveList.Count);
                float randomEnemyCost = _mobAggressiveList[randomEnemyID].Data.WaveCost;

                if (_waveValue - randomEnemyCost >= 0) {
                    generatedEnemies.Add(_mobAggressiveList[randomEnemyID]);
                    _waveValue -= randomEnemyCost;
                }
                else if (_waveValue <= 0) {
                    break;
                }
            }

            _generatedWave.Clear();
            _generatedWave = generatedEnemies;

            EventManager.Instance.OnWaveStart?.Invoke(_waveCount - _currentWave);
            _state = WaveManagerStates.InWave;
        }

        public void RemoveFromSpawnedList(GameObject obj) {
            _spawnedEnemies.Remove(obj);
        }
    }
}

