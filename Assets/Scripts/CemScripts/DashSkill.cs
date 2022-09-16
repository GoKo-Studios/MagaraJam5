using System.Collections;
using UnityEngine;

public class DashSkill : PlayerSkills
{
    [SerializeField] private float attackCooldown = 12.0f;
    private float attackCooldownCounter = 0.0f;
    private Player player;

    private void Start(){
        player = GetComponent<Player>();
        player.dashCollidedWithEnemy += HitSomeone;
    }

    private void HitSomeone(){
        Debug.Log("HELLO");
        if(IsAttackAvailable()){
            attackCooldownCounter = attackCooldown;
            StartCoroutine(AttackCooldownCountodwn());
        }
    }

    public bool IsAvailable(){
        if(cooldownCounter  == 0.0f){
            return true;
        }
        else{
            return false;
        }
    }

    public bool IsAttackAvailable(){
        if(attackCooldownCounter  == 0.0f){
            return true;
        }
        else{
            return false;
        }
    }
        
    public void OnUse(){
        if(IsAttackAvailable()){
            skillDamageValue = skillDamage; 
        }
        else{
            skillDamageValue = 0.0f;
        }

        cooldownCounter = skillCooldown;
        StartCoroutine(CooldownCountodwn());
    }

    private IEnumerator CooldownCountodwn(){
        while(cooldownCounter > 0.0f){
            yield return new WaitForSecondsRealtime(1.0f);
            cooldownCounter -= 1.0f;
        }
        cooldownCounter = Mathf.Clamp(cooldownCounter, 0.0f, skillCooldown);
        yield return null;
    }

    private IEnumerator AttackCooldownCountodwn(){
        while(attackCooldownCounter > 0.0f){
            yield return new WaitForSecondsRealtime(1.0f);
            attackCooldownCounter -= 1.0f;
        }
        attackCooldownCounter = Mathf.Clamp(attackCooldownCounter, 0.0f, attackCooldown);
        yield return null;
    }
    
    private void OnDestroy(){
        player.dashCollidedWithEnemy -= HitSomeone;
    }

}
