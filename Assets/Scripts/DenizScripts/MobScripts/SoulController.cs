using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Controllers;

public class SoulController : MonoBehaviour
{
    private Transform _target;
    private PoolableObjectController _poolableController;
    [SerializeField] private float _followSpeed;
    [SerializeField] private float _poolingTime;

    private void Awake() {
        _poolableController = GetComponent<PoolableObjectController>();
    }

    private void FixedUpdate() {
        if (_target != null)
        transform.position = Vector3.Slerp(transform.position, _target.position, _followSpeed * Time.fixedDeltaTime);
    }

    public void SetTarget(Transform Target) {
        _target = Target;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            AudioManagerC.Instance.Play("OrbPickUp");
            _poolableController.EnqueueCheck(_poolingTime);
            other.GetComponent<SoulCollector>().CollectSoul();
        }
    }
}
