using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;
using Assets.Scripts.Enums;

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
            foreach (Animator mesh in GetComponentsInChildren<Animator>()) {
                if (_manager.Data.Mesh.GetComponent<MeshTag>().Tag == mesh.gameObject.GetComponent<MeshTag>().Tag) {
                    _animator = mesh;
                }
            }
        }

        private void OnAnimation(string Name, MobAnimationControllerTypes Type, bool Value) {
            if (_animator == null) return;

            switch(Type) {
                case MobAnimationControllerTypes.Trigger:
                    _animator.SetTrigger(Name);
                break;

                case MobAnimationControllerTypes.Bool:
                    _animator.SetBool(Name, Value);
                break;
            }
        }
    }
}

