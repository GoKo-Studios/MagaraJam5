using System.Collections;
using UnityEngine;

public class SmashSkill : PlayerSkills
{

    public bool IsAvailable(){
        if(cooldownCounter == 0){
            return true;
        }
        else{
            return false;
        }
    }

    public void OnUse(){
        if(IsAvailable()){
            cooldownCounter = skillCooldown;
            StartCooldownCoroutine();
        }
        
    }
}
