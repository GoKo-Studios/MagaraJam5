using UnityEngine;

public class InputListener : MonoBehaviour
{
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

        if(Input.GetMouseButtonDown(1)){
            InputEventSystem.Instance.toTheFloor?.Invoke();
        }

        if(Input.GetMouseButtonUp(1)){
            InputEventSystem.Instance.toTheFloor?.Invoke();
        }
    }
}
