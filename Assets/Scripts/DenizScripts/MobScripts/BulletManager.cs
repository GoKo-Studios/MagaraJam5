using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Controllers;

namespace Assets.Scripts.Managers {
    public class BulletManager : MonoBehaviour
    {   
        [SerializeField] private float _speed;
        [SerializeField] private float _damage;
        [SerializeField] private float _poolingTime;
        [SerializeField] private float _despawnTime;
        private PoolableObjectController _poolableController;
        private TrailRenderer _trailRenderer;
        private float _timeElapsed = 0f;
        private Rigidbody _rigidBody;
        //private BulletMeshController _meshController;
        private bool _isActive = true;

        private void Awake() {
            _poolableController = GetComponent<PoolableObjectController>();
            _rigidBody = GetComponent<Rigidbody>();
            _trailRenderer = GetComponentInChildren<TrailRenderer>();
        }

        private void OnEnable() {
            _isActive = true;
            _timeElapsed = 0f;
            _trailRenderer.emitting = true;
        }

        private void OnDisable() {
            _isActive = false;
            _timeElapsed = 0f;
            _trailRenderer.emitting = false;
        }

        private void Start() {
            _timeElapsed = 0f;
        }

        private void FixedUpdate() {
            Move();
        }

        private void Update() {
            if (!_isActive) return;

            _timeElapsed += Time.deltaTime;
            if (_timeElapsed >= _despawnTime) {
                Destroy();
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (other.transform.gameObject.tag == "Player") {
                other.GetComponent<PlayerHealthManager>().TakeDamage(_damage);
                Destroy();
            }
        }

        private void Move() {
            _rigidBody.MovePosition(transform.position + transform.forward * _speed * Time.fixedDeltaTime);
        }

        private void Destroy() {
            _trailRenderer.emitting = false;
            _trailRenderer.Clear();
            _poolableController.EnqueueCheck(_poolingTime);
        }

        public void Setup() {

        }

        public void Clear() {

        }
    }
}

