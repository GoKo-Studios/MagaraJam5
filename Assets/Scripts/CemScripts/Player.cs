using UnityEngine;
using System.Collections;
using Cinemachine;
using Assets.Scripts.Managers;
using Assets.Scripts.Params;
using UnityEngine.AI;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public Vector3 movingDirection{get; private set;}
    private CharacterController charController;
    [SerializeField] private float walkingSpeed = 5.0f;
    [SerializeField] private float runningSpeed = 8.5f;
    [SerializeField] private float dashSpeed = 50.0f;
    [SerializeField] private float dashDuration = 0.1f;
    [SerializeField] private float smashSpeed = 30.0f;
    [SerializeField] private float smashHangingTime = 0.3f;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float gravity = -29.43f; // 3 * -9.81
    [SerializeField] private float fallingSpeed = 1.0f;
    
    private Vector3 velocity;
    public bool isGrounded{get; private set;}

    private Transform groundCheck;

    private CinemachineVirtualCamera playerFollowCam;

    private PlayerInputManager playerInput;

    private theArrowMovement arrowMovement;

    public PlayerStates playerStates = PlayerStates.Idle;

    private bool[] stateAwake = new bool[6];
    private int stateSize = 6;

    private float dashTimer = 0.0f;

    public float distanceFromGround{get; private set;}

    public LayerMask groundLayerMask;

    public LayerMask enemyLayerMask;

    private DashSkill dashSkill;
    private SmashSkill smashSkill;
    private PullSkill pullSkill;

    public UnityAction dashCollidedWithEnemy;
    public UnityAction playerJumpEvent;
    public UnityAction onSmashEnter;
    public UnityAction onSmashExit;
    
    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        playerInput = PlayerInputManager.Instance;
        groundCheck = transform.GetChild(0).transform;
        playerFollowCam = GameObject.Find("PlayerFollowCam").GetComponent<CinemachineVirtualCamera>();
        arrowMovement = GameObject.Find("Noname Weapon").GetComponent<theArrowMovement>();

        dashSkill = GetComponent<DashSkill>();
        smashSkill = GetComponent<SmashSkill>();
        pullSkill = GetComponent<PullSkill>();

        for(int i = 0; i < stateSize; i++){
            stateAwake[i] = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundMask);
        isGrounded = charController.isGrounded;
        
        Vector3 forwardMovingDir = playerFollowCam.transform.forward;
        forwardMovingDir.y = 0.0f;
        movingDirection = (forwardMovingDir * playerInput.movingInput.y + playerFollowCam.transform.right * playerInput.movingInput.x).normalized;

        RaycastHit ray;
        if(Physics.Raycast(transform.position, Vector3.down, out ray, 50f, groundLayerMask)){
            distanceFromGround = ray.distance;
        } 

        switch (playerStates){
            case PlayerStates.Idle:
                IdleStateMovement();
                break;
            case PlayerStates.Walking:
                relocationStateMovement(walkingSpeed);
                break;
            case PlayerStates.Running:
                relocationStateMovement(runningSpeed);
                break;
            case PlayerStates.Aiming:
                AimingMovement();
                break;
            case PlayerStates.Dashing:
                DashStateMovement();
                break;
            case PlayerStates.GroundSmash:
                GroundSmashStateMovement();                
                break;
        }

        playerInput.setInputsToFalse();
    }

    private void IdleStateMovement(){
        if(stateAwake[0] == true){
            //first transition frame.
            setAllAwakes();
            //playerInput.setInputsToFalse();
            stateAwake[1] = false;
        }
        if(playerInput.movingInput.magnitude > 0.0f){
            playerStates = PlayerStates.Walking;
        }
        
        applyPlayerYVelocity();

        PullEnemiesSkill();

        if(arrowMovement.theArrowState == theArrowMovement.ArrowStates.CallBack){
            LookAtMouse();
        }

        if(distanceFromGround > 3.0f && playerInput.groundSmashEvent && smashSkill.IsAvailable()){
            playerStates = PlayerStates.GroundSmash;
            smashSkill.OnUse();
        }
    }

    private void relocationStateMovement(float speed){

        if(stateAwake[1] == true){
            //first transition frame.
            setAllAwakes();
            stateAwake[1] = false;
        }

         
        charController.Move(movingDirection * speed * Time.deltaTime);

        if(arrowMovement.theArrowState == theArrowMovement.ArrowStates.OutAndActive){
            if(movingDirection != Vector3.zero){
                LookAtMovementRotation();
            }
        }
        else if(arrowMovement.theArrowState == theArrowMovement.ArrowStates.CallBack){
            LookAtMouse();                  
        }

        applyPlayerYVelocity();

        PullEnemiesSkill();

        if(playerInput.runEvent){
            playerStates = PlayerStates.Running;
        }
        else{
            playerStates = PlayerStates.Walking;
        }

        if(playerInput.dashEvent && dashSkill.IsAvailable()){
            dashSkill.OnUse();
            playerStates = PlayerStates.Dashing;
        }
        else if(playerInput.groundSmashEvent && distanceFromGround > 3.0f && smashSkill.IsAvailable()){
            playerStates = PlayerStates.GroundSmash;
            smashSkill.OnUse();
        }
        else{
            if(movingDirection.magnitude == 0 && isGrounded){
                StartCoroutine(setState(PlayerStates.Idle, 0.0f));
            }
        }

    }

    private void AimingMovement(){
        if(stateAwake[1] == true){
            //first transition frame.
            setAllAwakes();
            stateAwake[1] = false;
        }

        charController.Move(movingDirection * walkingSpeed * Time.deltaTime);
    }

    private void DashStateMovement(){
        if(stateAwake[3] == true){
            dashTimer = Time.time;
            setAllAwakes();
            stateAwake[3] = false;
        }
        charController.Move(movingDirection * dashSpeed * Time.deltaTime);

        if(dashSkill.IsAttackAvailable()){
            Collider[] colliders = Physics.OverlapSphere(transform.position, 1.4f);
            foreach(Collider col in colliders){
                if(col.transform.tag == "Enemy"){
                    MobManager mobManager = col.GetComponentInParent<MobManager>();
                    Vector3 direction = (col.transform.position - transform.position).normalized;
                    direction.y = 1.0f;
                    
                    Rigidbody rb = col.GetComponentInParent<Rigidbody>();
                    
                    if(mobManager.isActiveAndEnabled){
                        mobManager.OnHit?.Invoke(new MobOnHitParams(mobManager, dashSkill.skillDamageValue, 
                        dashSkill.getKnockbackAmount(), direction, dashSkill.getStunDuration(), rb));
                    }

                    dashCollidedWithEnemy?.Invoke();

                }
            }
        }

        //playerInput.dashEvent = false;
        if(Time.time - dashTimer > dashDuration){
            playerStates = PlayerStates.Idle;
        }
    }

    private void GroundSmashStateMovement(){
        if(stateAwake[4] == true){
            onSmashEnter?.Invoke();
            smashHangingTime = Time.time;
            setAllAwakes();
            stateAwake[4] = false;
        }

        bool hitTheGround = false;

        //playerInput.groundSmashEvent = false;

        if(isGrounded && !hitTheGround){

            hitTheGround = true;

            Collider[] colliders = Physics.OverlapSphere(transform.position, 5.0f, enemyLayerMask);
            if(colliders.Length != 0){
                foreach(Collider col in colliders){
                    if(col.GetComponentInParent<MobManager>()){
                        try{
                            MobManager mobManager = col.GetComponentInParent<MobManager>();
                            Vector3 direction = (col.transform.position - transform.position).normalized;
                            direction.y = 1.0f;
                            
                            Rigidbody rb = col.GetComponentInParent<Rigidbody>();
                            
                            if(mobManager.isActiveAndEnabled){
                                mobManager.OnHit?.Invoke(new MobOnHitParams(mobManager, smashSkill.skillDamageValue,
                                 smashSkill.getKnockbackAmount(), direction, smashSkill.getStunDuration(), rb));
                            }
                        }
                        catch{

                        }
                    }
                }
            }
            CameraManager.Instance.ScreenShake();
            StartCoroutine(setState(PlayerStates.Idle, 0.5f));
        }
        else if(Time.time - smashHangingTime > 0.3f){
            charController.Move(Vector3.down * smashSpeed * Time.deltaTime);
        }
        else{
            charController.Move(Vector3.zero * Time.deltaTime);
        }
    }

    private void applyPlayerYVelocity(){
        if(isGrounded && velocity.y < 0f){
        velocity.y = -2.0f;
        }

        if(playerInput.jumpEvent && isGrounded){
            velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            PlayerInputManager.Instance.jumpEvent = false;
            playerJumpEvent?.Invoke();
        }

        velocity.y += gravity * fallingSpeed * Time.deltaTime;
        charController.Move(velocity * Time.deltaTime);
    }

    private void PullEnemiesSkill(){
        if(playerInput.pullEnemiesEvent && isGrounded && arrowMovement.taggedEnemyList.Count > 0){
            foreach(Transform enemy in arrowMovement.taggedEnemyList){
                if(enemy == null){
                    return;
                }
                try{
                    MobManager mb = enemy.GetComponentInParent<MobManager>();

                    mb.OnStunned?.Invoke(mb, pullSkill.getStunDuration());
                    
                    Transform tf = mb.GetComponent<Transform>();
                    NavMeshAgent agent = enemy.GetComponentInParent<NavMeshAgent>();
                    StartCoroutine(PullEnemies((transform.position - tf.position).normalized, tf, agent));
                }
                catch{
                    arrowMovement.taggedEnemyList.Remove(enemy);
                }
            }
            arrowMovement.OnListClear?.Invoke();
            arrowMovement.taggedEnemyList.Clear();
        }
    }

    private IEnumerator PullEnemies(Vector3 dir, Transform tf, NavMeshAgent agent){
        while(Vector3.Distance(transform.position, tf.position) > 3.0f){
            yield return new WaitForFixedUpdate();
            agent.Move(dir * Time.deltaTime * pullSkill.getPullSpeed());
        }
        yield return null;
    }
    
    private IEnumerator setState(PlayerStates state, float delay){
        yield return new WaitForSecondsRealtime(delay);
        if(playerStates == PlayerStates.GroundSmash){
            onSmashExit?.Invoke();
        }
        playerStates = state;
    }

    private void LookAtMovementRotation(){
        Quaternion toRotation = Quaternion.LookRotation(movingDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 10.0f * Time.deltaTime);
    }

    private void LookAtMouse(){
        Vector3 normal = (InputListener.mousePosOnWorld - transform.position).normalized;
        float angle = Mathf.Atan2(normal.x, normal.z) * Mathf.Rad2Deg;

        //Prevents sudden rotation when mouse goes out off border.
        if (InputListener.mousePosOnWorld != Vector3.zero){
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    private void setAllAwakes(){
        for(int i = 0; i < stateSize; i++){
            stateAwake[i] = true;
        }
    }


}
