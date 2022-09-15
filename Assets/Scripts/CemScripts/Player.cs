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

    public CinemachineVirtualCamera playerFollowCam;

    private PlayerInputManager playerInput;
    
    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        playerInput = PlayerInputManager.Instance;
        groundCheck = transform.GetChild(0).transform;
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

    
}
