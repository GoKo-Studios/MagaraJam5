using UnityEngine;
using Assets.Scripts.Managers;
using Assets.Scripts.Enums;

namespace Assets.Scripts.Controllers {

    public class PoolableObjectController : MonoBehaviour
    {
        [HideInInspector] public bool IsCalledByPooling;
        [SerializeField] private PoolableObjectTypes _type;

        // If the object were to wait for an animation, timer logic should be handled in the respective object script instead of here.
        public void EnqueueCheck(float Timer)
        {   
            if (IsCalledByPooling)
            {
               Invoke(nameof(Enqueue), Timer);
            }
        }

        private void Enqueue()
        {
            IsCalledByPooling = false;
            ObjectPoolingManager.Instance.EnqueueObject(gameObject, _type);
        }
    }
}

