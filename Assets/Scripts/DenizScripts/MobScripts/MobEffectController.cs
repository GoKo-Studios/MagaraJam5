using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Controllers {
    public class MobEffectController : MonoBehaviour
    {
        private MobManager _manager;
        [SerializeField] private GameObject _attackIndicator;
        [SerializeField] private Shader _fillShader;
        private Material _attackIndicatorMaterial;

        private void Awake() {
            _manager = GetComponentInParent<MobManager>();
        }

        private void OnEnable() {
            _manager.OnSetup += OnSetup;
            _manager.OnEffect += OnEffect;

            _manager.SpawnAttackIndicator += SpawnAttackIndicator;
            _manager.DespawnAttackIndicator += DespawnAttackIndicator;
            _manager.UpdateAttackIndicator += UpdateAttackIndicator;
        }

        private void OnDisable() {
            _manager.OnSetup -= OnSetup;
            _manager.OnEffect -= OnEffect;
        }

        private void OnSetup() {
            _attackIndicator.transform.localScale = _manager.Data.AttackAreaSize;
            _attackIndicator.GetComponent<MeshRenderer>().material = new Material(_fillShader);
            _attackIndicator.SetActive(false);
        }

        private void OnEffect() {

        }

        private void SpawnAttackIndicator() {
            _attackIndicator.SetActive(true);
        }

        private void DespawnAttackIndicator() {
            _attackIndicator.SetActive(false);
        }

        private void UpdateAttackIndicator(float Progress) {
            if (_attackIndicatorMaterial == null) return;
            _attackIndicatorMaterial.SetFloat("_ProgressBorder", Progress);
        }
    }
}

public enum EffectTypes {
    AttackIndicator
}

