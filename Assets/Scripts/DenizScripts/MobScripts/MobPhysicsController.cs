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
       private BoxCollider _trigger;

       private void Awake() {
            _manager = GetComponentInParent<MobManager>();
            _rigidbody = GetComponentInParent<Rigidbody>();
            _trigger = GetComponent<BoxCollider>();
       }

       private void OnEnable() {
            _manager.OnSetup += OnSetup;
       }

       private void OnDisable() {
            _manager.OnSetup -= OnSetup;
       }

       private void OnTriggerEnter(Collider other) {
            if (other.tag != "Weapon") return;

            // Rewrite here when this variables are implemented into the weapon script.
            WeaponDamageHolder holder = other.gameObject.GetComponent<WeaponDamageHolder>();
            Vector3 direction = -(other.transform.position - transform.position).normalized;
            _manager.OnHit?.Invoke(new MobOnHitParams(_manager, holder.Damage, holder.Knockback, direction, holder.StunDuration, _rigidbody));
       }

       private void OnSetup() {
        _trigger.size = _manager.Data.TriggerSize;
       }
    }
}

