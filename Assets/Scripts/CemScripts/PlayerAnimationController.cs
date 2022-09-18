using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{

    private Player player;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<Player>();
        animator = GetComponent<Animator>();
        player.onSmashEnter += OnSmashEvent;
        player.onSmashExit += OnSmashExit;
        player.onPullEvent += OnPullEvent;
    }

    void OnDestroy(){
        player.onSmashEnter -= OnSmashEvent;
        player.onSmashExit -= OnSmashExit;
        player.onPullEvent -= OnPullEvent;
    }

    private void OnPullEvent(){
        animator.SetTrigger("PullTrigger");
    }

    private void OnSmashEvent(){
        animator.SetTrigger("SmashTrigger");
    }

    private void OnSmashExit(){
        animator.SetTrigger("BackToIdle");
    }

}
