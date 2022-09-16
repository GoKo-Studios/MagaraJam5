using UnityEngine;

public class InputListener : MonoBehaviour
{

    private float dashKeyPressTime;
    private float dashKeyReleaseTime;
    private bool isDashKeyReleased;
    private float runTimer = 0.0f;

    void Start(){
        //timer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Horizontal") || Input.GetButton("Vertical")){
            InputEventSystem.Instance.playerDirectionInput?.Invoke(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized);
        }
        else{
            InputEventSystem.Instance.playerDirectionInput?.Invoke(Vector2.zero);
        }

        if(Input.GetButtonDown("Jump")){
            InputEventSystem.Instance.JumpEvent?.Invoke();
        }

        DashKeyBehaviour();

        RunKeyBehaviour();

        if(Input.GetKeyDown(KeyCode.E)){
            InputEventSystem.Instance.groundSmash?.Invoke();
        }

        if(Input.GetKeyUp(KeyCode.LeftShift)){
            InputEventSystem.Instance.runEvent?.Invoke(false);
        }

        if(Input.GetKeyDown(KeyCode.F)){
            InputEventSystem.Instance.callSecondaryBack?.Invoke();
        }

        if(Input.GetMouseButtonDown(1)){
            InputEventSystem.Instance.toTheFloor?.Invoke();
        }

        if(Input.GetMouseButtonUp(1)){
            InputEventSystem.Instance.toTheFloor?.Invoke();
        }


    }

    private void DashKeyBehaviour(){
        if(Input.GetKeyDown(KeyCode.LeftShift)){
            dashKeyPressTime = Time.time;
        }

        if(Input.GetKeyUp(KeyCode.LeftShift)){
            dashKeyReleaseTime = Time.time;

            if(dashKeyReleaseTime - dashKeyPressTime < 0.2f){
                InputEventSystem.Instance.dashEvent?.Invoke();
            }
        }

    }

    private void RunKeyBehaviour(){
        if(Input.GetKeyDown(KeyCode.LeftShift)){
            runTimer = 0.0f;
        }

        if(Input.GetKey(KeyCode.LeftShift)){
            runTimer += Time.deltaTime;
            if(runTimer > 0.22f){
                InputEventSystem.Instance.runEvent?.Invoke(true);
            }
        }
    }
}
