using UnityEngine;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Controllers {

    public class PoolableObjectController : MonoBehaviour
    {
        [HideInInspector] public bool IsCalledByPooling;

        // If the object were to wait for an animation, timer logic should be handled in the respective object script instead of here.
        public void EnqueueCheck(float timer)
        {
            if (IsCalledByPooling)
            {
               Invoke(nameof(Enqueue), timer);
            }
        }

        private void Enqueue()
        {
            IsCalledByPooling = false;
            ObjectPoolingManager.Instance.EnqueueObject(gameObject);
        }
    }
}

