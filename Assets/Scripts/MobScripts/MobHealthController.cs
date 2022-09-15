using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Controllers {
    public class MobHealthController : MonoBehaviour
    {
        private MobManager _manager;
        private float _currentHealth;

        private void Awake() {
            _manager = GetComponent<MobManager>();
        }

        private void OnEnable() {
            _manager.OnSetup += OnSetup;
            //_manager.OnDamageTaken += OnDamageTaken;
        }

        private void OnDisable() {
            _manager.OnSetup -= OnSetup;
            //_manager.OnDamageTaken -= OnDamageTaken;
        }

        private void OnSetup() {
            //_currentHealth = _manager.MobData.MaxHealth;
        }

        private void OnDamageTaken(float DamageTaken) {
            
        }
    }
}

