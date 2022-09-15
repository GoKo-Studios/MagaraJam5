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

    private YonduArrow ya;
    
    private Transform player;

    [SerializeField] private AnimationCurve turnCurve;

    private Vector3 mousePos;

    private float curveTurnTime;
    private float curveDistanceTime;

    private float distance_mousePos_arrow;
    private float angleBetweenTarget;

    [SerializeField]
    private float arrowCenterDeadzone = 3.0f;

    public LayerMask layerMask;

    private Vector3 floorPosition;

    ArrowInputManager arrowInput;

    public enum ArrowStates { CallBack, OutAndActive, Disabled };
    public ArrowStates theArrowState = ArrowStates.OutAndActive;

    // Start is called before the first frame update
    void Start()
    {
        heightFromGround = transform.position.y;
        arrowInput = ArrowInputManager.Instance;

        rb = transform.GetComponent<Rigidbody>();
        ya = transform.GetComponent<YonduArrow>();

        player = GameObject.Find("CamFollow").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        switch (theArrowState){
            case ArrowStates.Disabled:
                
                return;
            case ArrowStates.OutAndActive:
                OutAndActiveMovement();
                return;
            case ArrowStates.CallBack:
                CallBackMovement();
                return;
        }

        
    }

    private void OutAndActiveMovement(){

        mousePos = YonduArrow.mosueWorldPosition();

        if(arrowInput.arrowToTheFloor){
            RaycastHit hit;
            if(Physics.Raycast(transform.position, Vector3.down, out hit, 50.0f, layerMask)){
                mousePos.y = hit.transform.position.y;
            }
        }

        RotateArrow(mousePos);

        moveArrow(mousePos, arrowCenterDeadzone);

        if(arrowInput.callArrowBack){
            arrowInput.callArrowBack = false;
            theArrowState = ArrowStates.CallBack;
        }
    }

    private void CallBackMovement(){

        RotateArrow(player.position);

        moveArrow(player.position, 1.0f);

        if(arrowInput.callArrowBack){
            arrowInput.callArrowBack = false;
            theArrowState = ArrowStates.OutAndActive;
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
        float mappedDistance = YonduArrow.MapRange(stopRange, distanceCap, 0.0f, 30.0f, clampedDistance);
        // Debug.Log(Vector3.Distance(transform.position, mousePos));
        rb.velocity = transform.forward * speed * TurnSpeedCurve(angleBetweenTarget) * mappedDistance;
    }

    private float TurnSpeedCurve(float angle) {
        float res = YonduArrow.MapRange(0, 180, 0, 1, angle);
        curveTurnTime = res;
        return turnCurve.Evaluate(res);
    }


}
