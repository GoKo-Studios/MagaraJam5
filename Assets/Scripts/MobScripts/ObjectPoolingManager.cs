using System.Collections.Generic;
using UnityEngine;

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

        [SerializeField] private int _initialAmount; 
        private Queue<GameObject> _poolableObjectQueue = new Queue<GameObject>();

        //[SerializeField] private List<Queue<GameObject>> _poolableObjectQueue = new List<Queue<GameObject>>();

        [SerializeField] private GameObject _mobPrefab; // Only temporary. Could be loaded from Resources.

        public void Start()
        {
            Setup();
        }

        private void Setup()
        {   
            for (int x = 0; x < _initialAmount; x++) {
                AddObject();
            } 
        }

        public void EnqueueObject(GameObject obj)
        {
            // These can be put into a function for ease of use.
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;
            obj.gameObject.SetActive(false);
        }

        public GameObject DequeObject()
        {
            // If there are no remaining poolable objects, initialize more.
            if (_poolableObjectQueue.Count <= 0) {
                AddObject();
            }

            GameObject obj = _poolableObjectQueue.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        private void AddObject() {
            GameObject obj = Instantiate(_mobPrefab, transform, true);
            _poolableObjectQueue.Enqueue(obj);
            obj.SetActive(false);
        }
    }
}


