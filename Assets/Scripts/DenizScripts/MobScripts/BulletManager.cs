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
        private PoolableObjectController _poolableController;
        private Rigidbody _rigidBody;
        //private BulletMeshController _meshController;

        private void Awake() {
            _poolableController = GetComponent<PoolableObjectController>();
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate() {
            Move();
        }

        private void OnTiggerEnter(Collider other) {
            if (other.tag == "Player") {
                other.GetComponentInParent<PlayerHealthManager>().TakeDamage(_damage);
            }
            Destroy();
        }

        private void Move() {
            _rigidBody.MovePosition(transform.position + transform.forward * _speed * Time.fixedDeltaTime);
        }

        private void Destroy() {
            _poolableController.EnqueueCheck(_poolingTime);
        }

        public void Setup() {

        }

        public void Clear() {

        }
    }
}

