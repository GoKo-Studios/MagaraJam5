using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Params {
    public struct MobOnAttackHitParams
    {
        public MobOnAttackHitParams(Collider[] Target, float DamageDealt) {
            this.Target = Target;
            this.DamageDealt = DamageDealt;
        }

        Collider[]  Target;
        public float DamageDealt;
    }
}
