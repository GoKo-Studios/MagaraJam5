using UnityEngine;

[System.Serializable]
public struct MobData
{
    public string DisplayName;
    public int MaxHealth;
    public int FollowSpeed;
    public int DetectionRange;
    public int AttackRange;
    public int StoppingDistance;
    public int DamageDealt;
    //public int JumpHeight;
    public LayerMask DetectionLayerMask;
    public GameObject Mesh;
    public int WaveCost;
    public int PoolingTime;
    public float AttackTime;
}