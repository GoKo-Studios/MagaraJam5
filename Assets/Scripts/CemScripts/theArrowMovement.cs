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

    public YonduArrow ya;

    [SerializeField] private AnimationCurve distanceCurve;
    [SerializeField] private AnimationCurve turnCurve;

    private Vector3 mousePos;

    private float curveTurnTime;
    private float curveDistanceTime;

    private float distance_mousePos_arrow;
    private float angle_mousePos_arrow;

    [SerializeField]
    private float arrowCenterDeadzone = 3.0f;

    public LayerMask layerMask;

    private Vector3 floorPosition;

    ArrowInputManager arrowInput;

    // Start is called before the first frame update
    void Start()
    {
        heightFromGround = transform.position.y;
        arrowInput = ArrowInputManager.Instance;

        rb = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = ya.mosueWorldPosition();
        
        distance_mousePos_arrow = Vector3.Distance(transform.position, mousePos);

        if(arrowInput.arrowToTheFloor){
            RaycastHit hit;
            if(Physics.Raycast(transform.position, Vector3.down, out hit, 50.0f, layerMask)){
                mousePos.y = hit.transform.position.y;
            }
        }

        RotateArrow();

        moveArrow();
        
    }

    private void RotateArrow(){
        Vector3 relativePos = (new Vector3(mousePos.x, mousePos.y + heightFromGround, mousePos.z) - transform.position).normalized;
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        angle_mousePos_arrow = Vector3.Angle(transform.forward, relativePos);
        transform.rotation = Quaternion.Lerp( transform.rotation, toRotation, rotateSpeed * Time.deltaTime );
    }

    private void moveArrow(){
        float clampedDistance = Mathf.Clamp(distance_mousePos_arrow, arrowCenterDeadzone, distanceCap);
        float mappedDistance = YonduArrow.MapRange(arrowCenterDeadzone, distanceCap, 0.0f, 30.0f, clampedDistance);
        // Debug.Log(Vector3.Distance(transform.position, mousePos));
        rb.velocity = transform.forward * speed * TurnSpeedCurve(angle_mousePos_arrow) * mappedDistance;
    }
    
    private float DistanceCurve(float distance) {
        float res = YonduArrow.MapRange(0, 30, 0, 1, distance);
        curveDistanceTime = res;
        return distanceCurve.Evaluate(res);
    }

    private float TurnSpeedCurve(float angle) {
        float res = YonduArrow.MapRange(0, 180, 0, 1, angle);
        curveTurnTime = res;
        return turnCurve.Evaluate(res);
    }


}
