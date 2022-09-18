using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingsAnimController : MonoBehaviour
{   
    Player player;
    void Start(){
        player = GetComponentInParent<Player>();
        player.onSmashEnter += OnSmashEvent;
    }

    void OnDestroy(){
        player.onSmashEnter -= OnSmashEvent;
    }

    private void OnSmashEvent(){

    }

    private void OnDashEvent(){

    }
}
