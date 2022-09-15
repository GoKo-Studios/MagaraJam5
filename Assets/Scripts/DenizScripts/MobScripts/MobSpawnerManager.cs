using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Controllers;

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
        }

        #endregion

        private Transform _target;
        // [SerializeField] private GameObject _spawnerLocations;
        // private List<SpawnPointData> _spawnerPoints = new List<SpawnPointData>();

        public GameObject SpawnObjectWithPooling(MobDataBase Data)
        {
            Vector3 position = new Vector3(0,0,0); //GetSpawnPosition();
            var spawnedObj = ObjectPoolingManager.Instance.DequeObject();
            spawnedObj.GetComponent<MobManager>().Setup(Data);

            if (spawnedObj.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent)) {
                agent.Warp(position);
            }
            else {
                spawnedObj.transform.position = position;
            }

            spawnedObj.GetComponent<PoolableObjectController>().IsCalledByPooling = true;
            return spawnedObj;
        }

        private Vector3 GetSpawnPosition() {
            float distance = Vector3.Distance(transform.position, _target.position);
            List<Vector3> availablePoints = new List<Vector3>();
            // foreach (SpawnPointData point in _spawnerPoints) {
            //     // REDO THIS CALCULATION
            //     if (point.Distance - distance <= _playerSpawnRange) {
            //         availablePoints.Add(point.Position);
            //     }
            // }
            int randomPointIndex = Random.Range(0, availablePoints.Count);
            return availablePoints[randomPointIndex];
        }

         // private void SetupSpawnerList() 
        // {   
        //     Vector3 position;
        //     float distance;
        //     foreach (Transform transform in _spawnerLocations.GetComponentInChildren<Transform>()) {
        //         position = transform.position;
        //         distance = Vector3.Distance(position, this.transform.position);
        //         _spawnerPoints.Add(new SpawnPointData(position, distance));
        //     }
        // }

        // private void OnDrawGizmos() {
        //     Gizmos.color = Color.red;
        //     if (_target != null) Gizmos.DrawWireSphere(_target.position, _playerSpawnRange);
        // }
    }
}

