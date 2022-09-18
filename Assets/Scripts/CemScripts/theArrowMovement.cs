using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class theArrowMovement : MonoBehaviour
{

    private Rigidbody rb;
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float distanceCap = 15.0f;
    [SerializeField] private float rotateSpeed = 10.0f;
    [SerializeField] private float shotSpeed = 50.0f;
    [SerializeField] private float shotAutoCallBackTime = 1.2f;
    private float shotCallBackTimer;

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

    public Transform aimPosTransform;

    private Player player;

    public List<Transform> taggedEnemyList;

    private bool setPositionDone;
    private bool setRotationDone;

    private WeaponDamageHolder weaponDamageHolder;

    public ArrowStates theArrowState = ArrowStates.CallBack;
    private bool[] stateAwake = new bool[4];
    private int stateSize = 4;

    public UnityEngine.Events.UnityAction<Transform> OnListAdd;
    public UnityEngine.Events.UnityAction<Transform> OnListRemove;
    public UnityEngine.Events.UnityAction OnListClear;

    // Start is called before the first frame update
    void Start()
    {
        heightFromGround = transform.position.y;
        arrowInput = ArrowInputManager.Instance;

        rb = transform.GetComponent<Rigidbody>();

        playerCallBackDestination = GameObject.Find("WeaponCallBackPos").transform;
        player = GameObject.Find("Player").GetComponent<Player>();
        playerMana = player.transform.GetComponent<PlayerMana>();
        weaponDamageHolder = transform.GetComponent<WeaponDamageHolder>();

        aimPosTransform = player.transform.GetChild(3).GetComponent<Transform>();

        for(int i = 0; i < stateSize; i++){
            stateAwake[i] = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        switch (theArrowState){
            case ArrowStates.OutAndActive:
                OutAndActiveMovement();
                break;
            case ArrowStates.CallBack:
                CallBackMovement();
                break;
            case ArrowStates.Aiming:
                AimingMovement();
                break;
            case ArrowStates.Charged:
                OnChargedMove();
                break;
        }

        if(theArrowState == ArrowStates.CallBack && Vector3.Distance(transform.position, playerCallBackDestination.position) < 2.5f){
            GetComponent<BoxCollider>().enabled = false;
        }
        else if(theArrowState == ArrowStates.Aiming){
            GetComponent<BoxCollider>().enabled = false;
        }
        else{
            GetComponent<BoxCollider>().enabled = true;
        }

        arrowInput.SetInputsToFalse();
    }

    private void OutAndActiveMovement(){
        if(stateAwake[1] == true){
            weaponDamageHolder.SetToWanderingValues();
            setAllAwakes();
            stateAwake[1] = false;
        }

        mousePos = InputListener.mousePosOnWorld;

        if(arrowInput.arrowToTheFloor){
            RaycastHit hit;
            if(Physics.Raycast(transform.position, Vector3.down, out hit, 50.0f, layerMask)){
                mousePos.y = hit.transform.position.y;
            }
        }

        RotateArrow(mousePos);

        moveArrow(mousePos, arrowCenterDeadzone);

        if(arrowInput.callArrowBack || !PlayerMana.Instance.ArrowAvailability()){
            theArrowState = ArrowStates.CallBack;
            CameraManager.Instance.setOrthoSize(11.0f);
        }

    }

    private void CallBackMovement(){
        if(stateAwake[0] == true){
            weaponDamageHolder.SetToWanderingValues();
            setAllAwakes();
            stateAwake[0] = false;
        }

        RotateArrow(playerCallBackDestination.position);

        moveArrow(playerCallBackDestination.position, 2.0f);

        Debug.Log(Vector3.Distance(transform.position, playerCallBackDestination.position));

        if(arrowInput.callArrowBack && PlayerMana.Instance.ArrowAvailability() && 
        Vector3.Distance(transform.position, playerCallBackDestination.position) < 5.0f ){
            theArrowState = ArrowStates.OutAndActive;
            CameraManager.Instance.setOrthoSize(13.5f);
        }

        if(arrowInput.aimEvent){
            theArrowState = ArrowStates.Aiming;
        }
    }

    private void AimingMovement(){
        if(stateAwake[2] == true){
            weaponDamageHolder.SetToWanderingValues();
            setAllAwakes();
            stateAwake[2] = false;
        }
        
        if(!setPositionDone){
            SetArrowPos(aimPosTransform.position, 2.0f); 
        }
        else{
            transform.position = aimPosTransform.position;
        }

        SetArrowRotation(player.transform, 5.0f);

        if(setPositionDone && setRotationDone){
            if(arrowInput.shootEvent){
                theArrowState = ArrowStates.Charged;
            }
            else if(arrowInput.aimEvent){
                theArrowState = ArrowStates.CallBack;
            }
        }
            
    }

    private void OnChargedMove(){
        
        if(stateAwake[3] == true){
            shotCallBackTimer = Time.time;
            weaponDamageHolder.SetToShotValues();
            setAllAwakes();
            stateAwake[3] = false;
        }
        if(Time.time - shotCallBackTimer > shotAutoCallBackTime){
            theArrowState = ArrowStates.CallBack;
        }
        else if(arrowInput.callArrowBack){
            theArrowState = ArrowStates.CallBack;
        }
        else{
            rb.velocity = transform.forward * shotSpeed;
        }
        
    }

    private void OnTriggerEnter(Collider other){
        Debug.Log(other.transform.name);
        if(other.tag == "Enemy"){
            if(!taggedEnemyList.Contains(other.transform)){
                taggedEnemyList.Add(other.transform);
                OnListAdd?.Invoke(other.transform);
            }
        }
    }

    public void RemoveFromTaggedEnemyList(Transform enemy) {
        if(taggedEnemyList.Contains(enemy)){
            taggedEnemyList.Remove(enemy);
            OnListRemove?.Invoke(enemy);
        }
    }

    private void RotateArrow(Vector3 destination) {
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

    private void SetArrowPos(Vector3 destination, float speed){
        transform.position = Vector3.Lerp(transform.position, destination, Time.deltaTime * speed);
        Debug.Log(destination);
        if(Vector3.Distance(transform.position, destination) < 0.1f){
            setPositionDone = true;
        }
        else{
            setPositionDone = false;
        }
    }

    private void SetArrowRotation(Transform destination, float speed){
        transform.forward = Vector3.Lerp(transform.forward, destination.forward, Time.deltaTime * speed);
        if(transform.forward == destination.forward){
            setRotationDone = true;
        }
        else{
            setRotationDone = false;
        }
    }

    private void setAllAwakes(){
        for(int i = 0; i < stateSize; i++){
            stateAwake[i] = true;
        }
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
