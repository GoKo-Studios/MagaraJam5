using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] private float _health;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _invulnerableTime;
    private bool _isInvulnerable = false;

    public void TakeDamage(float DamageTaken) {
        if (_isInvulnerable) return;
        StartCoroutine(HandleInvulnerableState());
        HandleDamage(DamageTaken);

    }

    private void HandleDamage(float DamageTaken) {
        _currentHealth -= DamageTaken;
        if (_currentHealth <= 0) {
            HandleDeath();
        }
    }

    private void HandleDeath() {
        EventManager.Instance.OnGameEnd?.Invoke();
    }

    private IEnumerator HandleInvulnerableState() {
        _isInvulnerable = true;
        yield return new WaitForSecondsRealtime(_invulnerableTime);
        _isInvulnerable = false;
    }

    private void Start() {
        _currentHealth = _health;
    }

    private void OnDisable() {
        StopAllCoroutines();
    }
}
