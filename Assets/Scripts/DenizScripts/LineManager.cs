using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;

public class LineManager : MonoBehaviour
{
    [SerializeField] private GameObject _linePrefab;
    [SerializeField] private theArrowMovement _arrowScript;
    [SerializeField] private List<Line> _lineList = new List<Line>();
    [SerializeField] private Queue<GameObject> _lineQueue = new Queue<GameObject>();
    [SerializeField] private int _lineInitialAmount;
    [SerializeField] private Player _player;
    private GameObject _circle;

    [Header("Line Renderer")]
    [SerializeField] private int _lineSteps; // Complexity of the circles.
    [SerializeField] private AnimationCurve _lineCurve; // Width of the circles.
    [SerializeField] private Material _lineMaterial; 
    [SerializeField] private float _radius;

    private void Awake() {
        _arrowScript = GetComponent<theArrowMovement>();
        _player = FindObjectOfType<Player>();
    }

    private void OnEnable() {
        _arrowScript.OnListAdd += OnListAdd;
        _arrowScript.OnListRemove += OnListRemove;
        _arrowScript.OnListClear += OnListClear;
    }

    private void OnDisable() {
        _arrowScript.OnListAdd -= OnListAdd;
        _arrowScript.OnListRemove -= OnListRemove;
        _arrowScript.OnListClear -= OnListClear;
    }

    void Start()
    {   
        DrawCircle(_lineSteps, _radius);
        for (int i = 0; i < _lineInitialAmount; i++) {
            AddLine();    
        }
    }

    private void Update() {
        if (_lineList.Count == 0) {
            DisableCircle();
        }
        else {
            EnableCircle();
        }
    }

    public void EnableLine(Transform target) {
        if (!target.parent.gameObject.activeInHierarchy) return;
        if (target.GetComponentInParent<MobManager>().MobData == null) return;

        bool contains = false;

        foreach(Line line in _lineList) {
            if (line.Target == target) {
                contains = true;
            }
        }

        if (contains) return;

        if (_lineQueue.Count <= 0) {
            AddLine();
        }
        GameObject obj = _lineQueue.Dequeue();
        SetupLine(obj.GetComponent<LineController>(), target);
        _lineList.Add(new Line(obj, target));
        obj.SetActive(true);
    }

    public void DisableLine(GameObject obj) {
        _lineQueue.Enqueue(obj);
        obj.SetActive(false);
    }

    public void DequeuLine(Transform target) {
        Line lineRef = new Line();
        foreach (Line line in _lineList) {
            if (line.Target == target) {
               lineRef = line;
            }
        }
        _lineList.Remove(lineRef);
        DisableLine(lineRef.Renderer);
    }

    private void AddLine() {
        GameObject obj = Instantiate(_linePrefab, transform);
        _lineQueue.Enqueue(obj);
        obj.SetActive(false);
    }

    private void SetupLine(LineController controller, Transform target) {
        controller.AssignTarget(_circle.transform, target);
    }

    private void OnListAdd(Transform obj) {
        EnableLine(obj);
    }

    private void OnListRemove(Transform obj) {
        DequeuLine(obj);
    }

    private void OnListClear() {
        foreach (Line line in _lineList) {
            DisableLine(line.Renderer);
        }
        _lineList.Clear();
    }

    private void EnableCircle() {
        _circle.SetActive(true);
    }

    private void DisableCircle() {
        _circle.SetActive(false);
    }

    private void DrawCircle(int steps, float radius) {
        _circle = Instantiate(new GameObject(), _player.transform.position, Quaternion.identity, _player.transform);
        LineRenderer lineRenderer = _circle.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.widthCurve = _lineCurve;
        lineRenderer.material = _lineMaterial;
        lineRenderer.positionCount = steps;
 
        for(int currentStep=0; currentStep<steps; currentStep++)
        {
            float circumferenceProgress = (float)currentStep/(steps-1);
 
            float currentRadian = circumferenceProgress * 2 * Mathf.PI;
            
            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);
 
            float x = radius * xScaled;
            float y = 0;
            float z = radius * yScaled;
 
            Vector3 currentPosition = new Vector3(x,y,z);
 
            lineRenderer.SetPosition(currentStep,currentPosition);
        }
    }
}

public struct Line {
    public Line(GameObject Renderer, Transform Target) {
        this.Renderer = Renderer;
        this.Target = Target;
    }

    public GameObject Renderer;
    public Transform Target;
}
