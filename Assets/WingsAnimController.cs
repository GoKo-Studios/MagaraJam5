using UnityEngine;

public class WingsAnimController : MonoBehaviour
{   
    Player player;
    Animator animator;
    private bool smashAnimation;
    private bool dashAnimation;

    void Start(){
        player = GetComponentInParent<Player>();
        animator = GetComponent<Animator>();
        player.onSmashEnter += OnSmashEvent;
        player.onSmashExit += OnSmashEvent;
        player.onDashEnter += OnDashEvent;
        player.onDashExit += OnDashEvent;
        smashAnimation = false;
        dashAnimation = false;
    }

    void OnDestroy(){
        player.onSmashEnter -= OnSmashEvent;
        player.onSmashExit -= OnSmashEvent;
        player.onDashEnter -= OnDashEvent;
        player.onDashExit -= OnDashEvent;
    }

    void Update(){
        animator.SetBool("Smashing", smashAnimation);
        animator.SetBool("Dashing", dashAnimation);
    }

    private void OnSmashEvent(){
        smashAnimation = !smashAnimation;
    }

    private void OnDashEvent(){
        dashAnimation = !dashAnimation;
    }
}
