using UnityEngine;

public class WeaponDamageHolder : MonoBehaviour
{

    public float Damage{get; private set;}
    public float Knockback{get; private set;}
    public float StunDuration{get; private set;}

    public float WanderingDamage = 25.0f;
    public float WanderingKnockback = 4.0f;
    public float WanderingStunDuration = 2.0f;

    public float ShotDamage = 25.0f;
    public float ShotKnockback = 4.0f;
    public float ShotStunDuration = 2.0f;

    public void SetToWanderingValues(){
        this.Damage = WanderingDamage;
        this.Knockback = WanderingKnockback;
        this.StunDuration = WanderingStunDuration;
    }

    public void SetToShotValues(){
        this.Damage = ShotDamage;
        this.Knockback = ShotKnockback;
        this.StunDuration = ShotStunDuration;
    }

}
