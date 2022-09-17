using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class theArrowMovement : MonoBehaviour
{

    private Rigidbody rb;
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float distanceCap = 15.0f;
    [SerializeField] private float rotateSpeed = 10.0f;

    private float heightFromGround;
    
    private Transform playerCallBackDestination;

    private PlayerMana playerMana;

    [SerializeField] private AnimationCurve turnCurve;

    private Vector3 mousePos;

    private float curveTurnTime;
    private float curveDistanceTime;

    private float distance_mousePos_arrow;
    private float angleBetweenTarget;

    [SerializeField]
    private float arrowCenterDeadzone = 3.0f;

    public LayerMask layerMask;

    public LayerMask mouseRayMask;

    private Vector3 floorPosition;

    ArrowInputManager arrowInput;

    public List<Transform> taggedEnemyList;

    public enum ArrowStates { CallBack, OutAndActive, Disabled };
    public ArrowStates theArrowState = ArrowStates.CallBack;

    // Start is called before the first frame update
    void Start()
    {
        heightFromGround = transform.position.y;
        arrowInput = ArrowInputManager.Instance;

        rb = transform.GetComponent<Rigidbody>();

        playerCallBackDestination = GameObject.Find("WeaponCallBackPos").transform;
        playerMana = GameObject.Find("Player").GetComponent<PlayerMana>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        switch (theArrowState){
            case ArrowStates.Disabled:
                
                break;
            case ArrowStates.OutAndActive:
                OutAndActiveMovement();
                break;
            case ArrowStates.CallBack:
                CallBackMovement();
                break;
        }

        arrowInput.SetInputsToFalse();
    }

    private void OutAndActiveMovement(){

        mousePos = InputListener.mousePosOnWorld;

        if(arrowInput.arrowToTheFloor){
            RaycastHit hit;
            if(Physics.Raycast(transform.position, Vector3.down, out hit, 50.0f, layerMask)){
                mousePos.y = hit.transform.position.y;
            }
        }

        RotateArrow(mousePos);

        moveArrow(mousePos, arrowCenterDeadzone);

        if(arrowInput.callArrowBack || !playerMana.ArrowAvailability()){
            theArrowState = ArrowStates.CallBack;
            CameraManager.Instance.setOrthoSize(11.0f);
        }
    }

    private void CallBackMovement(){

        RotateArrow(playerCallBackDestination.position);

        moveArrow(playerCallBackDestination.position, 2.0f);

        if(arrowInput.callArrowBack && playerMana.ArrowAvailability()){
            theArrowState = ArrowStates.OutAndActive;
            CameraManager.Instance.setOrthoSize(13.5f);
        }
    }

    private void OnTriggerEnter(Collider other){
        Debug.Log(other.transform.name);
        if(other.tag == "Enemy"){
            if(!taggedEnemyList.Contains(other.transform)){
                taggedEnemyList.Add(other.transform);
            }
        }
    }

    public void RemoveFromTaggedEnemyList(Transform enemy){
        if(taggedEnemyList.Contains(enemy)){
            taggedEnemyList.Remove(enemy);
        }
    }

    private void RotateArrow(Vector3 destination){
        Vector3 relativePos = (new Vector3(destination.x, destination.y + heightFromGround, destination.z) - transform.position).normalized;
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        angleBetweenTarget = Vector3.Angle(transform.forward, relativePos);
        transform.rotation = Quaternion.Lerp( transform.rotation, toRotation, rotateSpeed * Time.deltaTime );
    }

    private void moveArrow(Vector3 targetToMove, float stopRange){
        float distance = Vector3.Distance(transform.position, targetToMove);

        float clampedDistance = Mathf.Clamp(distance, stopRange, distanceCap);
        float mappedDistance = MapRange(stopRange, distanceCap, 0.0f, 30.0f, clampedDistance);
        // Debug.Log(Vector3.Distance(transform.position, mousePos));
        rb.velocity = transform.forward * speed * TurnSpeedCurve(angleBetweenTarget) * mappedDistance;
    }

    private float TurnSpeedCurve(float angle) {
        float res = MapRange(0, 180, 0, 1, angle);
        curveTurnTime = res;
        return turnCurve.Evaluate(res);
    }

    public static float MapRange(float input_start, float input_end, float output_start, float output_end, float value ){
        return output_start + ((output_end - output_start) / (input_end - input_start)) * (value - input_start);
    }

    public static float MapRange(float input_start, float input_end, float output_start, float slope, float value, bool optimized ){
        return output_start + slope * (value - input_start);
    }

}
