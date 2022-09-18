using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;

public class LevelEndTrigger : MonoBehaviour
{
    [SerializeField] private float _levelEndTime;
    private bool _isLoading = false;

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            StartCoroutine(NextLevel(_levelEndTime));
        }
    }
    
    private IEnumerator NextLevel(float Time) {
        if (_isLoading) yield break;
        _isLoading = true;
        EventManager.Instance.OnNextLevel();
    }
}
