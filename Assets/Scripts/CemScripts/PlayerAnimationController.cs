using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{

    private Player player;
    private CharacterController characterController;
    private Animator animator;
    private float forwardVelocity;
    private float rightVelocity;
    private float playerSpeed;
    private bool hasPlayerJumped;
    public LayerMask floorOrGrounLM;
    private Vector3 animationMovingDirection;
    private Vector3 distanceF;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<Player>();
        animator = GetComponent<Animator>();
        characterController = player.GetComponent<CharacterController>();
        player.playerJumpEvent += hasJumped;
        player.onSmashEnter += OnSmashEvent;
        player.onSmashExit += OnSmashExit;
    }

    void OnDestroy(){
        player.playerJumpEvent -= hasJumped;
        player.onSmashEnter -= OnSmashEvent;
        player.onSmashExit -= OnSmashExit;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.playerStates == PlayerStates.Running){
            playerSpeed = 2.0f;
        }
        else {
            playerSpeed = 1.0f;
        }
        animationMovingDirection = Vector3.Lerp(animationMovingDirection, player.movingDirection * playerSpeed, Time.deltaTime * 2.5f);

        forwardVelocity = Vector3.Dot(animationMovingDirection , player.transform.forward);
        rightVelocity = Vector3.Dot(animationMovingDirection, player.transform.right);

        animator.SetFloat("VelocityY", forwardVelocity);
        animator.SetFloat("VelocityX", rightVelocity);

        

        if(hasPlayerJumped){
            hasPlayerJumped = false;
            animator.SetTrigger("Jump");
        }

        // if(player.isGrounded && animator.GetCurrentAnimatorStateInfo(0).IsName("Player Fall")){
        //     animator.SetTrigger("BackToBlendTree");
        // }

        if(player.distanceFromGround > 2.0f){
            animator.SetBool("FallBool", true);
        }
        else if(player.distanceFromGround < 2.0f){
            animator.SetBool("FallBool", false);
        }

        // switch(player.playerStates){
        //     case(PlayerStates.Walking):
            
                

        //         return;
        //     case(PlayerStates.Idle):
        //         forwardVelocity = Vector3.Dot(characterController.velocity, transform.forward);
        //         rightVelocity = Vector3.Dot(characterController.velocity, transform.right);

        //         Debug.Log(player.movingDirection);

        //         animator.SetFloat("VelocityY", forwardVelocity);
        //         animator.SetFloat("VelocityX", rightVelocity);
        //         return;
        // }

    }

    private void hasJumped(){
        hasPlayerJumped = true;
    }

    private void OnSmashEvent(){
        animator.SetTrigger("SmashTrigger");
    }

    private void OnSmashExit(){
        animator.SetTrigger("BackToBlendTree");
    }

}
