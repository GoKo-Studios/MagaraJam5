using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Enums;

namespace Assets.Scripts.Managers {

    public class ObjectPoolingManager : MonoBehaviour
    {
        #region Singleton

        public static ObjectPoolingManager Instance;

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

        [SerializeField] private int _initialMobAmount;
        [SerializeField] private int _initialOrbAmount;
        private Queue<GameObject> _mobQueue = new Queue<GameObject>();
        private Queue<GameObject> _orbQueue = new Queue<GameObject>();

        [SerializeField] private List<Queue<GameObject>> _poolableObjectQueue = new List<Queue<GameObject>>();

        [SerializeField] private GameObject _mobPrefab; // Only temporary. Could be loaded from Resources.
        [SerializeField] private GameObject _orbPrefab;

        public void Start()
        {
            Setup();
        }

        private void Setup()
        {   
            for (int x = 0; x < _initialMobAmount; x++) {
                AddObject(PoolableObjectTypes.Mob);
            } 

            for (int x = 0; x < _initialOrbAmount; x++) {
                AddObject(PoolableObjectTypes.Orb);
            } 
        }

        public void EnqueueObject(GameObject obj, PoolableObjectTypes Type)
        {   
            switch (Type) {
                case PoolableObjectTypes.Mob:
                    _mobQueue.Enqueue(obj);
                break;

                case PoolableObjectTypes.Orb:
                    _orbQueue.Enqueue(obj);
                break;
            }

            // These can be put into a function for ease of use.
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;
            obj.gameObject.SetActive(false);
        }

        public GameObject DequeObject(PoolableObjectTypes Type)
        {   
            GameObject obj = null;
            switch (Type) {
                case PoolableObjectTypes.Mob:
                    // If there are no remaining poolable objects, initialize more.
                    if (_mobQueue.Count <= 0) {
                        AddObject(PoolableObjectTypes.Mob);
                    }

                    obj = _mobQueue.Dequeue();
                    obj.SetActive(true);
                
                break;

                case PoolableObjectTypes.Orb:
                    // If there are no remaining poolable objects, initialize more.
                    if (_orbQueue.Count <= 0) {
                        AddObject(PoolableObjectTypes.Orb);
                    }

                    obj = _orbQueue.Dequeue();
                    obj.SetActive(true);
                break;
            }
            return obj;
        }

        private void AddObject(PoolableObjectTypes Type) {
            GameObject obj;
            switch (Type) {
                case PoolableObjectTypes.Mob:
                    obj = Instantiate(_mobPrefab, transform, true);
                    _mobQueue.Enqueue(obj);
                    obj.SetActive(false);
                break;

                case PoolableObjectTypes.Orb:
                    obj = Instantiate(_orbPrefab, transform, true);
                    _orbQueue.Enqueue(obj);
                    obj.SetActive(false);
                break;
            }
        }
    }
}


