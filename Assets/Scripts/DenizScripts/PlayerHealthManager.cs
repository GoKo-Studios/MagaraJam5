using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;

public class PlayerHealthManager : MonoBehaviour
{

    #region //singleton pattern

    public static PlayerHealthManager Instance;

    private void Awake(){
        if(Instance != this && Instance != null){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    [SerializeField] private float _health;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _invulnerableTime;
    private bool _isInvulnerable = false;
    private bool _isAlive = true;

    public void TakeDamage(float DamageTaken) {
        if (_isInvulnerable) return;
        // StartCoroutine(HandleInvulnerableState());
        HandleDamage(DamageTaken);

    }

    private void HandleDamage(float DamageTaken) {
        _currentHealth -= DamageTaken;
        if (_currentHealth <= 0) {
            _currentHealth = 0.0f;
            StartCoroutine(HandleDeath(3.0f));
        }
    }

    private IEnumerator HandleDeath(float Time) {
        if(!_isAlive) yield break;;
        _isAlive = false;
        yield return new WaitForSecondsRealtime(Time);
        EventManager.Instance.OnGameEnd?.Invoke();
        EventManager.Instance.OnRestartLevel?.Invoke();
    }

    // private IEnumerator HandleInvulnerableState() {
    //     _isInvulnerable = true;
    //     yield return new WaitForSecondsRealtime(_invulnerableTime);
    //     _isInvulnerable = false;
    // }

    private void Start() {
        _currentHealth = _health;
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    public float getHealth(){
        return _currentHealth;
    }
}
