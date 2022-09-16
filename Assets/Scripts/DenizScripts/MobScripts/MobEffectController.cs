using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Controllers {
    public class MobEffectController : MonoBehaviour
    {
        private MobManager _manager;
        [SerializeField] private GameObject _attackIndicatorObject;
        //[SerializeField] private Shader _fillShader;
        private Material _attackIndicatorMaterial;
        [SerializeField] private ParticleSystem _attackIndicatorParticle;

        private void Awake() {
            _manager = GetComponentInParent<MobManager>();
            _attackIndicatorParticle  = _attackIndicatorObject.GetComponent<ParticleSystem>(); 
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
            _attackIndicatorObject.transform.localScale = _manager.Data.AttackAreaSize;
            //_attackIndicatorParticle.GetComponent<Renderer>().material = new Material(_fillShader);
            _attackIndicatorMaterial = _attackIndicatorObject.GetComponent<Renderer>().material;
        }

        private void OnEffect() {

        }

        private void SpawnAttackIndicator() {
            _attackIndicatorParticle.Play();
        }

        private void DespawnAttackIndicator() {
            _attackIndicatorParticle.Stop();
        }

        private void UpdateAttackIndicator(float Progress) {
            if (_attackIndicatorMaterial == null) return;
            _attackIndicatorMaterial.SetFloat("_FillAmount", Progress);
        }
    }
}

public enum EffectTypes {
    AttackIndicator
}

