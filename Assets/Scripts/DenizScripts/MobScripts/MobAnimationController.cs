using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Controllers {
    public class MobAnimationController : MonoBehaviour
    {
        private Animator _animator;
        private MobManager _manager;

        private void Awake() {
            _manager = GetComponentInParent<MobManager>();
        }

        private void OnEnable() {
            _manager.OnSetup += OnSetup;
            _manager.OnAnimation += OnAnimation;
        }

        private void OnDisable() {
            _manager.OnSetup -= OnSetup;
            _manager.OnAnimation -= OnAnimation;
        }

        private void OnSetup() {
            foreach (GameObject mesh in GetComponentsInChildren<GameObject>()) {
                if (_manager.Data.Mesh.GetComponent<MeshTag>().Tag == mesh.GetComponent<MeshTag>().Tag) {
                    _animator = mesh.GetComponent<Animator>();
                }
            }
        }

        private void OnAnimation(string Trigger) {
            if (_animator == null) return;
            //_animator.SetTrigger();
        }
    }
}

