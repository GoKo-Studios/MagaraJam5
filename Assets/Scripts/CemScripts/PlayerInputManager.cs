using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    #region //singleton pattern

    public static PlayerInputManager Instance;

    private void Awake(){
        if(Instance != this && Instance != null){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    public Vector2 movingInput{get; private set;}
    public bool jumpEvent;

    private void Start(){
        InputEventSystem.Instance.playerDirectionInput += getMovementDirInput;
        InputEventSystem.Instance.JumpEvent += OnJumpevent;
    }
    
    private void OnDestroy(){
        InputEventSystem.Instance.playerDirectionInput -= getMovementDirInput;
        InputEventSystem.Instance.JumpEvent -= OnJumpevent;
    }

    protected void getMovementDirInput(Vector2 input){
        movingInput = input;
    }

    private void OnJumpevent(){
        jumpEvent = true;
    }

}
