using UnityEngine;
using System.Collections;

public abstract class PlayerSkills : MonoBehaviour
{
    [SerializeField] protected float skillCooldown;
    public float cooldownCounter {get; protected set;} = 0.0f;
    [SerializeField] protected float skillDamage;
    public float skillDamageValue{get; protected set;}
    [SerializeField] private float skillStunDuration;
    [SerializeField] private float skillKnockbackAmount;
    protected bool isAvailable;

    public virtual void SetAvailable(bool availability){
        isAvailable = availability;
    }

    protected void StartCooldownCoroutine(){
        StartCoroutine(CooldownCountdown());
    }

    private IEnumerator CooldownCountdown(){
        while(cooldownCounter > 0.0f){
            yield return new WaitForSecondsRealtime(1.0f);
            cooldownCounter -= 1.0f;
        }
        cooldownCounter = Mathf.Clamp(cooldownCounter, 0.0f, skillCooldown);
        yield return null;
    }

    public float getStunDuration(){
        return skillStunDuration;
    }
    public float getKnockbackAmount(){
        return skillKnockbackAmount;
    }

}
