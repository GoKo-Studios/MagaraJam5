using UnityEngine;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Params {
    public struct MobOnAttackParams
    {
        public MobOnAttackParams(MobManager Manager, Transform Target, float DamageDealt, float AttackRange, float AttackTime, 
        LayerMask Mask, Transform MobTransform, Vector3 AttackAreaSize) {
            this.Manager = Manager;
            this.Target = Target;
            this.DamageDealt = DamageDealt;
            this.AttackRange = AttackRange;
            this.AttackTime = AttackTime;
            this.Mask = Mask;
            this.MobTransform = MobTransform;
            this.AttackAreaSize = AttackAreaSize;
        }

        public MobManager Manager;
        public Transform Target;
        public float DamageDealt;
        public float AttackRange;
        public float AttackTime;
        public LayerMask Mask;
        public Transform MobTransform;
        public Vector3 AttackAreaSize;
    }
}
