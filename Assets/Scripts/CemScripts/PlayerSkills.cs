using UnityEngine;

public abstract class PlayerSkills : MonoBehaviour
{
    [SerializeField] protected float skillCooldown;
    public float cooldownCounter {get; protected set;} = 0.0f;
    [SerializeField] protected float skillDamage;
    public float skillDamageValue{get; protected set;}
    protected bool isAvailable;

    public virtual void SetAvailable(bool availability){
        isAvailable = availability;
    }

}
