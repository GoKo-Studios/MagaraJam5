using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Enums;
using Assets.Scripts.Controllers;

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

        [SerializeField] private int _currentWave;
        [SerializeField] private float _waveValue;
        [SerializeField] private int _timeBetweenWaves;
        private List<MobDataBase> _generatedWave = new List<MobDataBase>();
        [SerializeField] private List<GameObject> _spawnedEnemies;
        [SerializeField] private float _waveDuration;
        private float _waveTimer;
        private float _spawnInterval;
        private float _spawnTimer;
        private float _betweenTimer;
        [SerializeField] private WaveManagerStates _state;

        private void OnEnable() {
            //EventManager.Instance.OnGameStart += OnGameStart;
            //EventManager.Instance.OnWaveSkip += OnWaveSkip;
            //EventManager.Instance.OnGamePause += OnGamePause;
        }

        private void OnDisable() {
            //EventManager.Instance.OnPlay -= OnPlay;
            //EventManager.Instance.onSkip -= OnSkip;
        }

         void Start()
        {
            PopulateDataLists();
            SpawnPassiveMobs();
        }

         private void PopulateDataLists() {
            _mobPassiveList.Clear();
            _mobPassiveList.AddRange(ResourceLoader.LoadResources<MobDataBase>("Objects/PoolableObjects/Passive"));

            _mobAggressiveList.Clear();
            _mobAggressiveList.AddRange(ResourceLoader.LoadResources<MobDataBase>("Objects/PoolableObjects/Aggressive"));
        }

        void Update()
        {
            switch (_state) {
                case WaveManagerStates.InWave:
                    if (_spawnTimer <= 0) {
                        if (_generatedWave.Count > 0) {
                            GameObject obj = MobSpawnerManager.Instance.SpawnMobWithPooling(_generatedWave[0]);
                            _generatedWave.RemoveAt(0);
                            _spawnTimer = _spawnInterval;

                            _spawnedEnemies.Add(obj);
                        }
                        else {
                            _waveTimer = 0;
                        }   
                    }
                    else {
                        _spawnTimer -= Time.deltaTime;
                        _waveTimer -= Time.deltaTime;
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
                MobDataBase data = _mobPassiveList[Random.Range(0, _mobAggressiveList.Count)];
                GameObject obj = MobSpawnerManager.Instance.SpawnMobWithPoolingAndLocation(data, location);
                
            }
        }

        private void EndWave() {
            EventManager.Instance.OnWaveEnd?.Invoke();
            _state = WaveManagerStates.BetweenWave;
        }

        public void ProocedToNextWave() {
            _state = WaveManagerStates.Idle;
            _betweenTimer = _timeBetweenWaves;
            _currentWave++;
            GenerateWave();
        }

        private void GenerateWave() {
            _waveValue = _currentWave * 10;
            GenerateMobs();

            _spawnInterval = _waveDuration / _generatedWave.Count;
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

            EventManager.Instance.OnWaveStart?.Invoke(_currentWave);
            _state = WaveManagerStates.InWave;
        }

        public void RemoveFromSpawnedList(GameObject obj) {
            _spawnedEnemies.Remove(obj);
        }
    }
}

