using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Params {
    public struct MobOnMoveParams
    {
        public MobOnMoveParams(MobManager Manager, NavMeshAgent NavAgent, Transform MobTransform) {
            this.Manager = Manager;
            this.NavAgent = NavAgent;
            this.MobTransform = MobTransform;
        }

        public MobManager Manager;
        public NavMeshAgent NavAgent;
        public Transform MobTransform;
    }
}