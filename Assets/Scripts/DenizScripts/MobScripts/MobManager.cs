using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using Assets.Scripts.Controllers;
using Assets.Scripts.Params;
using Assets.Scripts.Enums;
using UnityEngine.AI;

namespace Assets.Scripts.Managers {
    public class MobManager : MonoBehaviour
    {
        [SerializeField] public MobDataBase MobData;
        [SerializeField] public MobStates State;
        public MobData Data;

        public float CurrentHealth;
        public bool IsAlert = false;

        #region References

        private MobPhysicsController _physicsController;
        private MobAnimationController _animationController;

        #endregion

        #region Events
        public UnityAction OnSetup;
        public UnityAction OnClear;

        public UnityAction<MobOnHitParams> OnHit => MobData.OnHit;
        public UnityAction<MobOnMoveParams> OnMove => MobData.MovementController;
        public UnityAction<NavMeshAgent, Vector3> MoveToPosition => MobData.MoveToPosition;
        public UnityAction<MobManager, float> OnDamageTaken => MobData.OnDamageTaken;
        public UnityAction<MobManager> OnDeath => MobData.OnDeath;
        public UnityAction<MobManager, float> OnStunned => MobData.OnStunned;
        public UnityAction<MobOnAttackHitParams> OnAttackHit => MobData.OnAttackHit;
        public UnityAction<MobOnAttackParams> OnAttack => MobData.OnAttack; 
        public UnityAction OnEnterRange;
        public UnityAction OnExitRange;

        #endregion

        public void Setup(MobDataBase ModData) {
            if (State != MobStates.NoData) return;
  
            this.MobData = Instantiate(ModData);
            Data = MobData.Data;

            //I wish I had a better solution in mind :(
            CurrentHealth = Data.MaxHealth;

            SetState(MobStates.Idle);
            OnSetup?.Invoke();
        }

        public void Clear() {
            SetState(MobStates.NoData);
            MobData = null;
            OnClear?.Invoke();
        }

        public void SetState(MobStates State) {
            this.State = State;
        }

        public MobStates GetState() => State;
    }

    // public UnityAction OnAttackMiss;
    // public UnityAction OnWait;
    // public UnityAction OnLoseTarget;
    // public UnityAction OnFollowTarget;
    // public UnityAction OnSearchTarget;
    // public UnityAction OnIdle;
}

