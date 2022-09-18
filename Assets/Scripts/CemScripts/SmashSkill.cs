
public class SmashSkill : PlayerSkills
{
    void Start(){
        skillDamageValue = skillDamage;
    }

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

    public float getCooldownCounter(){
        return cooldownCounter;
    }
}
