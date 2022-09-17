using UnityEngine;

public class UIDynamicVariables : MonoBehaviour
{

    #region //singleton pattern
    

    public static UIDynamicVariables Instance;

    private void Awake(){
        if(Instance != this && Instance != null){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    Transform playerTransform;
    DashSkill dashSkill;
    SmashSkill smashSkill;

    public float playerHP{get; private set;}
    public float playerHPSmooth{get; private set;}
    public float playerMana{get; private set;}
    public float playerDashCooldown{get; private set;}
    public float playerAttackDashCooldown{get; private set;}
    public float playerSmashCooldown{get; private set;}

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        dashSkill = playerTransform.GetComponent<DashSkill>();
        smashSkill = playerTransform.GetComponent<SmashSkill>();
    }

    void Update()
    {
        playerHP = PlayerHealthManager.Instance.getHealth();
        playerHPSmooth = Mathf.Lerp(playerHPSmooth, PlayerHealthManager.Instance.getHealth(), Time.deltaTime * 3.0f);

        playerMana = PlayerMana.Instance.getCurrentMana();

        playerDashCooldown = dashSkill.getDashCooldownCounter();
        playerAttackDashCooldown = dashSkill.getAttackCooldownCounter();

        playerSmashCooldown = smashSkill.getCooldownCounter();
    }



}
