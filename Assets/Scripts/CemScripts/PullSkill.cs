using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullSkill : MonoBehaviour
{
    [SerializeField] private float pullSpeed;
    [SerializeField] private float stunDuration;

    public float getPullSpeed(){
        return pullSpeed;
    }

    public float getStunDuration(){
        return stunDuration;
    }
}
