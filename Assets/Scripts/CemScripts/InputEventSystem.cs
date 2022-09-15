using UnityEngine;
using UnityEngine.Events;

public class InputEventSystem : MonoBehaviour
{
    #region //singleton pattern

    public static InputEventSystem Instance;

    private void Awake(){
        if(Instance != this && Instance != null){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    public delegate void pMovInput(Vector2 v);
    public pMovInput playerDirectionInput;

    public UnityAction JumpEvent;
    public UnityAction callSecondaryBack;
    public UnityAction toTheFloor;

}
