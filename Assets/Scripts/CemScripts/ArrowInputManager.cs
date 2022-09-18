using UnityEngine;

public class ArrowInputManager : MonoBehaviour
{
    #region //singleton pattern

    public static ArrowInputManager Instance;

    private void Awake(){
        if(Instance != this && Instance != null){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    public bool arrowToTheFloor;
    public bool callArrowBack;
    public bool aimEvent;
    public bool shootEvent;

    void Start()
    {
        arrowToTheFloor = false; //It's important for this bool to have false value in the beginning
        InputEventSystem.Instance.toTheFloor += toTheFloorEvent;
        InputEventSystem.Instance.callSecondaryBack += callArrowBackEvent;
        InputEventSystem.Instance.AimEvent += AimEventListener;
        InputEventSystem.Instance.ShootEvent += OnShootEvent;
    }

    void OnDestroy(){
        InputEventSystem.Instance.toTheFloor -= toTheFloorEvent;
        InputEventSystem.Instance.callSecondaryBack -= callArrowBackEvent;
        InputEventSystem.Instance.AimEvent -= AimEventListener;
        InputEventSystem.Instance.ShootEvent -= OnShootEvent;
    }

    private void toTheFloorEvent(){
        arrowToTheFloor = !arrowToTheFloor;
    }

    private void callArrowBackEvent(){
        callArrowBack = true;
    }

    public void AimEventListener(){
        aimEvent = true;
    }

    public void OnShootEvent(){
        shootEvent = true;
    }

    public void SetInputsToFalse(){
        callArrowBack = false;
        aimEvent = false;
        shootEvent = false;
    }
}
