using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    [SerializeField]private float maxMana = 100.0f;
    [SerializeField] private float decreaseRate;
    private float currentMana = 100.0f;
    private theArrowMovement arrowMovement;
    private SoulCollector soulCollector;

    private void Start(){
        soulCollector = GetComponent<SoulCollector>();
        arrowMovement = GameObject.Find("Noname Weapon").GetComponent<theArrowMovement>();
        soulCollector.OnSoulCollect += OnSoulCollect;
    }

    private void OnDestroy(){
        soulCollector.OnSoulCollect -= OnSoulCollect;
    }

    private void Update(){
        if(arrowMovement.theArrowState == theArrowMovement.ArrowStates.OutAndActive){
            currentMana -= decreaseRate * Time.deltaTime;
            currentMana = Mathf.Clamp(currentMana, 0.0f, maxMana);
        }

    }

    private void OnSoulCollect(){
        currentMana += 30.0f;
        if(currentMana >= 100.0f){
            currentMana = 100.0f;
        }
    }

    public bool ArrowAvailability(){
        if(currentMana >= 10.0f){
            return true;
        }
        else{
            return false;
        }
    }

}
