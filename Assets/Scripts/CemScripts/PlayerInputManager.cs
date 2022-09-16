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
    public bool runEvent;
    public bool dashEvent;
    public bool groundSmashEvent;
    public bool pullEnemiesEvent;

    private void Update(){
        if(jumpEvent){
            Debug.Log("Jump Event!");
        }
        if(runEvent){
            Debug.Log("Run Event!");
        }
        if(dashEvent){
            Debug.Log("Dash Event!");
        }
    }

    private void LateUpdate(){
        jumpEvent = false;
        dashEvent = false;
        groundSmashEvent = false;
        pullEnemiesEvent = false;
    }

    private void Start(){
        InputEventSystem.Instance.playerDirectionInput += getMovementDirInput;
        InputEventSystem.Instance.JumpEvent += OnJumpevent;
        InputEventSystem.Instance.runEvent += OnRunEvent;
        InputEventSystem.Instance.dashEvent += OnDashEvent;
        InputEventSystem.Instance.groundSmash += OnGroundSmashEvent;
        InputEventSystem.Instance.pullEnemies += OnPullEnemiesEvent;
    }
    
    private void OnDestroy(){
        InputEventSystem.Instance.playerDirectionInput -= getMovementDirInput;
        InputEventSystem.Instance.JumpEvent -= OnJumpevent;
        InputEventSystem.Instance.runEvent -= OnRunEvent;
        InputEventSystem.Instance.dashEvent -= OnDashEvent;
        InputEventSystem.Instance.groundSmash -= OnGroundSmashEvent;
        InputEventSystem.Instance.pullEnemies -= OnPullEnemiesEvent;
    }

    protected void getMovementDirInput(Vector2 input){
        movingInput = input;
    }

    private void OnJumpevent(){
        jumpEvent = true;
    }

    private void OnRunEvent(bool runInput){
        runEvent = runInput;
    }

    private void OnDashEvent(){
        dashEvent = true;
    }

    private void OnGroundSmashEvent(){
        groundSmashEvent = true;
    }

    private void OnPullEnemiesEvent(){
        pullEnemiesEvent = true;
    }

    public void setInputsToFalse(){
        dashEvent = false;
        groundSmashEvent = false;
        jumpEvent = false;
    }

}
