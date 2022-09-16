using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulCollector : MonoBehaviour
{
    [SerializeField] private float CurrentSouls;
    [SerializeField] private float AttractionRange;
    [SerializeField] private LayerMask SoulLayerMask;

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, AttractionRange, SoulLayerMask);
        foreach (Collider collider in colliders) {
            var controller = collider.gameObject.GetComponent<SoulController>();
            controller.SetTarget(transform);
        }
    }

    public void CollectSoul() {
        CurrentSouls++;
    }
}
