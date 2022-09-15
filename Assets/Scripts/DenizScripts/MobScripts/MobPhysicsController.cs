using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;
using Assets.Scripts.Params;

namespace Assets.Scripts.Controllers {
    public class MobPhysicsController : MonoBehaviour
    {
       private MobManager _manager;
       private Rigidbody _rigidbody;

       private void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
       }

       private void OnTriggerEnter(Collider other) {
            if (other.tag != "Weapon") return;
            //other.gameObject.GetComponent<>();
            Vector3 direction = (other.transform.position - transform.position).normalized;
            //_manager.MobData.OnHit?.Invoke(new MobOnHitParams(_manager, ));
       }
    }
}

