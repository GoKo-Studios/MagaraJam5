using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimationController : MonoBehaviour
{
    private Vector3 _lastForward;
    private Vector3 _currentForward;
    private float _angle;
    private Transform _mesh;
    [SerializeField] private float _roll;
    [SerializeField] private float _turn;
    [SerializeField] private float _angleTreshold;
    [SerializeField] private float _lerp;
    private float _direction;

    private void Awake() {
        _mesh = transform.GetChild(0).transform;
    }

    void Update()
    {   
        TurnAroundAxis();
        HandleRoll();

        _currentForward = transform.forward;

        _angle = Vector3.SignedAngle(_currentForward, _lastForward, Vector3.up);

        if (Mathf.Abs(_angle) >= _angleTreshold) {
            _direction = Mathf.Sign(_angle);
        }
        else {
            _direction = 0;
        }
        
        _lastForward = _currentForward;
    }

    private void HandleRoll() {
        if (_roll == 0) return;
        _mesh.localRotation = Quaternion.Lerp(_mesh.localRotation, Quaternion.Euler(0, 0, _roll * _direction), _lerp * Time.deltaTime);
    }

    private void TurnAroundAxis() {
        if (_turn == 0) return;
        _mesh.Rotate(new Vector3(0, 0, _turn * Time.deltaTime), Space.Self);   
    }
}
