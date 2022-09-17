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
        for (int i = 0; i < _lineInitialAmount; i++) {
            AddLine();    
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
        controller.AssignTarget(_player.transform, target);
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
}

public struct Line {
    public Line(GameObject Renderer, Transform Target) {
        this.Renderer = Renderer;
        this.Target = Target;
    }

    public GameObject Renderer;
    public Transform Target;
}
