using UnityEngine;
using UnityEngine.UI;

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

    public Slider SevapOrb;
    public Slider HealthOrb;
    private bool isCoolDown1 = false;
    private bool isCoolDown2 = false;
    public Image CoolDown1;
    public Image CoolDown2;
    private float cooldowntime;
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        dashSkill = playerTransform.GetComponent<DashSkill>();
        smashSkill = playerTransform.GetComponent<SmashSkill>();
    }

    void CoolDown()
    {
        if (playerDashCooldown >= 1 && isCoolDown1 == false )
        {
            cooldowntime = playerDashCooldown;
            isCoolDown1 = true;
            CoolDown1.fillAmount = 1; 
        }

        if (isCoolDown1)
        {
            CoolDown1.fillAmount -= 1 / cooldowntime * Time.deltaTime;
            if (CoolDown1.fillAmount <= 0)
            {
                CoolDown1.fillAmount = 0;
                isCoolDown1 = false;
            }
        }
        if (playerSmashCooldown >= 2 && isCoolDown2 == false)
        {
            cooldowntime = playerSmashCooldown;
            isCoolDown2 = true;
            CoolDown2.fillAmount = 1;
        }

        if (isCoolDown2)
        {
            CoolDown2.fillAmount -= 1 / cooldowntime * Time.deltaTime;
            if (CoolDown2.fillAmount <= 0)
            {
                CoolDown2.fillAmount = 0;
                isCoolDown2 = false;
            }
        }
    }
    void Update()
    {
        playerHP = PlayerHealthManager.Instance.getHealth();
        playerHPSmooth = Mathf.Lerp(playerHPSmooth, PlayerHealthManager.Instance.getHealth(), Time.deltaTime * 3.0f);

        playerMana = PlayerMana.Instance.getCurrentMana();

        playerDashCooldown = dashSkill.getDashCooldownCounter();
        playerAttackDashCooldown = dashSkill.getAttackCooldownCounter();

        playerSmashCooldown = smashSkill.getCooldownCounter();

        CoolDown();

        HealthOrb.value = playerHPSmooth;
        SevapOrb.value = playerMana;
    }



}
