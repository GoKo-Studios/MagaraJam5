using UnityEngine.Events;
using UnityEngine;

public class SoulCollector : MonoBehaviour
{
    [SerializeField] private float CurrentSouls;
    [SerializeField] private float AttractionRange;
    [SerializeField] private LayerMask SoulLayerMask;

    public UnityAction OnSoulCollect;

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, AttractionRange, SoulLayerMask);
        foreach (Collider collider in colliders) {
            var controller = collider.gameObject.GetComponent<SoulController>();
            controller.SetTarget(transform);
        }
    }

    public void CollectSoul() {
        OnSoulCollect?.Invoke();
        CurrentSouls++;
    }
}
