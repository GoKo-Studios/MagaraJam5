using System.Collections;
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
    
    public Image ClickedEffect1;
    public Image ClickedEffect2;
    public Image ClickedEffect3;
    public Image ClickedEffect4;
    private float cooldowntime;
    public float speed = 0.3f;
    public Color StartBlinkColor = Color.white;
    public Color EndBlinkColor = Color.gray;
    private GameObject DashAttackState;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        dashSkill = playerTransform.GetComponent<DashSkill>();
        smashSkill = playerTransform.GetComponent<SmashSkill>();
        DashAttackState = transform.Find("DashAttackState").gameObject;
    }

    void ClickEffect()
    {
        IEnumerator Blink1()
        {
            ClickedEffect1.color = Color.Lerp(StartBlinkColor, EndBlinkColor, speed);
            yield return new WaitForSeconds(0.2f);
            ClickedEffect1.color = Color.Lerp(EndBlinkColor, StartBlinkColor, speed);
            yield return null;
        }
        IEnumerator Blink2()
        {
            ClickedEffect2.color = Color.Lerp(StartBlinkColor, EndBlinkColor, speed);
            yield return new WaitForSeconds(0.2f);
            ClickedEffect2.color = Color.Lerp(EndBlinkColor, StartBlinkColor, speed);
            yield return null;
        }
        IEnumerator Blink3()
        {
            ClickedEffect3.color = Color.Lerp(StartBlinkColor, EndBlinkColor, speed);
            yield return new WaitForSeconds(0.2f);
            ClickedEffect3.color = Color.Lerp(EndBlinkColor, StartBlinkColor, speed);
            yield return null;
        }
        IEnumerator Blink4()
        {
            ClickedEffect4.color = Color.Lerp(StartBlinkColor, EndBlinkColor, speed);
            yield return new WaitForSeconds(0.2f);
            ClickedEffect4.color = Color.Lerp(EndBlinkColor, StartBlinkColor, speed);
            yield return null;
        }
        if (Input.GetKey(KeyCode.E))
        {
            
            StartCoroutine(Blink1());
            

        }
        if (Input.GetKey(KeyCode.LeftShift ))
        {
            StartCoroutine(Blink2());
        }
        if (Input.GetKey(KeyCode.Q))
        {
            StartCoroutine(Blink3());
        }
        if (Input.GetKey(KeyCode.F))
        {
            StartCoroutine(Blink4());
        }
        

    }
    void CoolDown()
    {
        if (playerDashCooldown >= 3 && isCoolDown1 == false )
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
        if (playerSmashCooldown >= 3 && isCoolDown2 == false)
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
        ClickEffect();
        
        if (dashSkill.IsAttackAvailable()){
            DashAttackState.SetActive(true);
        }
        else{
            DashAttackState.SetActive(false);
        }

        HealthOrb.value = playerHPSmooth;
        SevapOrb.value = playerMana;
    }



}
