using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    private Vector3 movingDirection;
    private CharacterController charController;
    [SerializeField] private float walkingSpeed = 5.0f;
    [SerializeField] private float runningSpeed = 8.5f;
    [SerializeField] private float dashSpeed = 25.0f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float gravity = -29.43f; // 3 * -9.81
    [SerializeField] private float fallingSpeed = 1.0f;
    private Vector3 velocity;
    public bool isGrounded{get; private set;}

    private Transform groundCheck;

    private CinemachineVirtualCamera playerFollowCam;

    private PlayerInputManager playerInput;

    private theArrowMovement arrowMovement;

    public enum PlayerStates{ Idle, Walking, Running, Dashing };
    public PlayerStates playerStates = PlayerStates.Walking;

    private bool[] stateAwake = new bool[4];
    private float stateSize = 4;

    private float dashTimer = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        playerInput = PlayerInputManager.Instance;
        groundCheck = transform.GetChild(0).transform;
        playerFollowCam = GameObject.Find("PlayerFollowCam").GetComponent<CinemachineVirtualCamera>();
        arrowMovement = GameObject.Find("Noname Weapon").GetComponent<theArrowMovement>();

        for(int i = 0; i < 4; i++){
            stateAwake[i] = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundMask);
        isGrounded = charController.isGrounded;

        

        switch (playerStates){
            case PlayerStates.Idle:
                IdleStateMovement();
                return;
            case PlayerStates.Walking:
                relocationStateMovement(walkingSpeed);
                return;
            case PlayerStates.Running:
                relocationStateMovement(runningSpeed);
                return;
            case PlayerStates.Dashing:
                DashStateMovement();
                return;
        }

    }

    private void IdleStateMovement(){
        if(playerInput.movingInput.magnitude > 0.0f){
            playerStates = PlayerStates.Walking;
        }
    }

    private void relocationStateMovement(float speed){

        if(stateAwake[1] == true){
            //first transition frame.
            setAllAwakes();
            stateAwake[1] = false;
        }

        Vector3 forwardMovingDir = playerFollowCam.transform.forward;
        forwardMovingDir.y = 0.0f;

        movingDirection = forwardMovingDir * playerInput.movingInput.y + playerFollowCam.transform.right * playerInput.movingInput.x; 
        charController.Move(movingDirection * speed * Time.deltaTime);

        if(arrowMovement.theArrowState == theArrowMovement.ArrowStates.OutAndActive){
                if(movingDirection != Vector3.zero){
                    LookAtMovementRotation();
            }
        }

        else if(arrowMovement.theArrowState == theArrowMovement.ArrowStates.CallBack){
            LookAtMouse();                  
        }

        if(isGrounded && velocity.y < 0f){
        velocity.y = -2.0f;
        }

        if(playerInput.jumpEvent && isGrounded){
            velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            PlayerInputManager.Instance.jumpEvent = false;
        }

        velocity.y += gravity * fallingSpeed * Time.deltaTime;
        charController.Move(velocity * Time.deltaTime);

        if(playerInput.runEvent){
            playerStates = PlayerStates.Running;
        }
        else{
            playerStates = PlayerStates.Walking;
        }

        if(playerInput.dashEvent){
            playerStates = PlayerStates.Dashing;
        }
    }

    private void DashStateMovement(){
        if(stateAwake[3] == true){
            dashTimer = Time.time;
            setAllAwakes();
            stateAwake[3] = false;
        }
        charController.Move(movingDirection * dashSpeed * Time.deltaTime);

        playerInput.dashEvent = false;
        if(Time.time - dashTimer > dashDuration){
            playerStates = PlayerStates.Idle;
        }
    }
    

    private void LookAtMovementRotation(){
        Quaternion toRotation = Quaternion.LookRotation(movingDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 10.0f * Time.deltaTime);
    }

    private void LookAtMouse(){
        Vector3 normal = (YonduArrow.mosueWorldPosition() - transform.position).normalized;
        float angle = Mathf.Atan2(normal.x, normal.z) * Mathf.Rad2Deg;

        //Prevents sudden rotation when mouse goes out off border.
        if (YonduArrow.mosueWorldPosition() != Vector3.zero){
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    private void setAllAwakes(){
        for(int i = 0; i < stateSize; i++){
            stateAwake[i] = true;
        }
    }
}
