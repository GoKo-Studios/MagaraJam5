using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimationController : MonoBehaviour
{
    private Vector3 _lastForward;
    private Vector3 _currentForward;
    private float _angle;
    private Transform _mesh;
    [SerializeField] private float _yaw;
    [SerializeField] private float _pitch;
    [SerializeField] private float _roll;
    [SerializeField] private float _turn;
    [SerializeField] private bool _isTurningAroundAxis;

    private void Awake() {
        _mesh = transform.GetChild(0).transform;
    }

    void Update()
    {   
        TurnAroundAxis();

        _currentForward = transform.forward;
        _angle = Vector3.SignedAngle(_currentForward, _lastForward, Vector3.up);
        Mathf.Sign(_angle);
        _lastForward = _currentForward;
    }

    private void TurnAroundAxis() {
        if (_isTurningAroundAxis)
        _mesh.Rotate(new Vector3(0, 0, _turn * Time.deltaTime), Space.Self);
    }
}
