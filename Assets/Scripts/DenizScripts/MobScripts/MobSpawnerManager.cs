using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Controllers;
using Assets.Scripts.Enums;

namespace Assets.Scripts.Managers {
    public class MobSpawnerManager : MonoBehaviour
    {
        #region Singleton

        public static MobSpawnerManager Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            SetupSpawnerList();

            _lastSpawnedPoint = new SpawnPointData(transform, 0);
        }

        #endregion

        private Transform _target;
        [SerializeField] private GameObject _spawnerLocations;
        private List<SpawnPointData> _spawnerPoints = new List<SpawnPointData>();
        private SpawnPointData _lastSpawnedPoint;

        public GameObject SpawnMobWithPooling(MobDataBase Data)
        {
            Transform location = GetSpawnPosition();
            var spawnedObj = ObjectPoolingManager.Instance.DequeObject(PoolableObjectTypes.Mob);
            spawnedObj.GetComponent<MobManager>().Setup(Data);

            if (spawnedObj.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent)) {
                agent.Warp(location.position);
            }
            else {
                spawnedObj.transform.position = location.position;
            }
            spawnedObj.transform.rotation = location.rotation;

            spawnedObj.GetComponent<PoolableObjectController>().IsCalledByPooling = true;
            return spawnedObj;
        }

        public GameObject SpawnOrbWithPooling(Vector3 Position) {
            var spawnedObj = ObjectPoolingManager.Instance.DequeObject(PoolableObjectTypes.Orb);
            spawnedObj.transform.position = Position;
            spawnedObj.GetComponent<PoolableObjectController>().IsCalledByPooling = true;
            return spawnedObj;
        }

        public GameObject SpawnBulletWithPooling(Vector3 Position, Vector3 Direction) {
            var spawnedObj = ObjectPoolingManager.Instance.DequeObject(PoolableObjectTypes.Bullet);
            spawnedObj.transform.position = Position;
            spawnedObj.transform.forward = Direction;
            spawnedObj.GetComponent<PoolableObjectController>().IsCalledByPooling = true;
            return spawnedObj;
        }

        private Transform GetSpawnPosition() {
            // Redo here if needed!

            List<SpawnPointData> availablePoints = new List<SpawnPointData>();
            int maxSpawnAmount = 0;

            foreach (SpawnPointData point in _spawnerPoints) {
                if (point.SpawnAmount >= maxSpawnAmount) maxSpawnAmount = point.SpawnAmount;
            }

            foreach (SpawnPointData point in _spawnerPoints) {
                if (point.SpawnAmount <= maxSpawnAmount) {
                    if (_lastSpawnedPoint.Location.position != point.Location.position)
                    availablePoints.Add(point);
                }
            }

            if (availablePoints.Count == 0) {
                Debug.Log("Spawned using random.");
                int randomPointIndex = Random.Range(0, _spawnerPoints.Count);
                var point = _spawnerPoints[randomPointIndex];
                point.SpawnAmount++;
                _lastSpawnedPoint = point;
                return point.Location;
            }
            else {
                Debug.Log("Spawned using available points.");
                int randomPointIndex = Random.Range(0, availablePoints.Count);
                var point = availablePoints[randomPointIndex];
                point.SpawnAmount++;
                _lastSpawnedPoint = point;
                return point.Location;
            }
        }

        private void SetupSpawnerList() 
        {   
            foreach (Transform transform in _spawnerLocations.GetComponentInChildren<Transform>()) {
                _spawnerPoints.Add(new SpawnPointData(transform, 0));
            }
        }

        // private void OnDrawGizmos() {
        //     Gizmos.color = Color.red;
        //     if (_target != null) Gizmos.DrawWireSphere(_target.position, _playerSpawnRange);
        // }
    }
}

