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

    void Start()
    {
        arrowToTheFloor = false; //It's important for this bool to have false value in the beginning
        InputEventSystem.Instance.toTheFloor += toTheFloorEvent;
    }

    void OnDestroy(){
        InputEventSystem.Instance.toTheFloor -= toTheFloorEvent;
    }

    private void toTheFloorEvent(){
        arrowToTheFloor = !arrowToTheFloor;
    }
}
