using UnityEngine;

[System.Serializable]
public struct MobData
{
    public string DisplayName;
    public int MaxHealth;
    public float FollowSpeed;
    public float DetectionRange;
    public float AttackRange;
    public float StoppingDistance;
    public float DamageDealt;
    //public int JumpHeight;
    public LayerMask DetectionLayerMask;
    public GameObject Mesh;
    public float WaveCost;
    public float PoolingTime;
    public float AttackTime;
    public Vector3 AttackAreaSize;
    public Vector3 TriggerSize;
    public float InvulnerableTime;
    public float AttackAngleTreshold;
    public float PanicDuration;
    public float OnDeathPanicRange;
    public float PanicRandomRange;
    public LayerMask PanicLayerMask;
    //public GameObject AttackIndicator;

    [Header("VFX")]
    public ParticleSystem HitEffect;

    [Header("SFX")]
    public AudioClip HitSound;
}
