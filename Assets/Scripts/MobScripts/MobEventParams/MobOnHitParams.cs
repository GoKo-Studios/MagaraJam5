using UnityEngine;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Params {
    public struct MobOnHitParams {

        public MobOnHitParams(MobManager Manager, float DamageTaken, float Knockback, Vector3 Direction, float StunDuration, Transform Source, Rigidbody Rb) {
            this.Manager = Manager;
            this.DamageTaken = DamageTaken;
            this.Knockback = Knockback;
            this.Direction = Direction;
            this.StunDuration = StunDuration;
            this.Source = Source;
            this.Rb = Rb;
        }

        public MobManager Manager;
        public float DamageTaken;
        public float Knockback;
        public Vector3 Direction;
        public float StunDuration;
        public Transform Source;
        public Rigidbody Rb;
    }
}
