using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    public CollectableSpawner Spawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            Destroy(gameObject);
        }
    }
}