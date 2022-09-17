using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    [SerializeField] private LineManager _manager;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _origin;

    private void Awake() {
        _lineRenderer = GetComponent<LineRenderer>();
        _manager = GetComponentInParent<LineManager>();
    }

    void Update()
    {
        if (_target == null) {
            _manager.DequeuLine(_target);
        }

        _lineRenderer.SetPosition(0, _origin.position);
        _lineRenderer.SetPosition(1, _target.position);
    }

    public void AssignTarget(Transform startPoint, Transform newTarget) {
        _lineRenderer.positionCount = 2;
        _origin = startPoint;
        _target = newTarget;
    }
}
