using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    private Vector3 movingDirection;
    private CharacterController charController;
    [SerializeField] private float movementSpeed = 5.0f;
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

    private float cameraZoomLerper = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        playerInput = PlayerInputManager.Instance;
        groundCheck = transform.GetChild(0).transform;
        playerFollowCam = GameObject.Find("PlayerFollowCam").GetComponent<CinemachineVirtualCamera>();
        arrowMovement = GameObject.Find("Noname Weapon").GetComponent<theArrowMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        //isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundMask);
        isGrounded = charController.isGrounded;

        Vector3 forwardMovingDir = playerFollowCam.transform.forward;
        forwardMovingDir.y = 0.0f;

        movingDirection = forwardMovingDir * playerInput.movingInput.y + playerFollowCam.transform.right * playerInput.movingInput.x; 
        charController.Move(movingDirection * movementSpeed * Time.deltaTime);

        switch (playerStates){
            case PlayerStates.Walking:
            if(arrowMovement.theArrowState == theArrowMovement.ArrowStates.OutAndActive){
                if(movingDirection != Vector3.zero){
                    LookAtMovementRotation();
                }
                cameraZoomLerper += 1.5f * Time.deltaTime;
                cameraZoomLerper = Mathf.Clamp(cameraZoomLerper, 0.0f, 1.0f);
            }
            else if(arrowMovement.theArrowState == theArrowMovement.ArrowStates.CallBack){
                LookAtMouse();
                cameraZoomLerper -= 1.5f * Time.deltaTime;
                cameraZoomLerper = Mathf.Clamp(cameraZoomLerper, 0.0f, 1.0f);
            }

            playerFollowCam.m_Lens.OrthographicSize = Mathf.Lerp(11.0f, 13.5f, cameraZoomLerper);

            if(isGrounded && velocity.y < 0f){
            velocity.y = -2.0f;
            }

            if(playerInput.jumpEvent && isGrounded){
                velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
                PlayerInputManager.Instance.jumpEvent = false;
            }

            velocity.y += gravity * fallingSpeed * Time.deltaTime;
            charController.Move(velocity * Time.deltaTime);

            return;
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
}
