using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Params;
using UnityEngine.AI;
using Assets.Scripts.Managers;
using Assets.Scripts.Enums;
using Assets.Scripts.Controllers;

[System.Serializable]
public abstract class MobDataBase : ScriptableObject
{
    // Any Mob specific variables must be internal. 
    // This wouldn't be a problem since Mob actions are also modified internally.

    public MobData Data;

    // All possible actions in MobManager must be included here.
    // Actions can be modified internally.

    #region Virtual Functions

    // Movement logic to be used in MobMovementController.
    public virtual void MovementController(MobOnMoveParams Params) {

            // Search for a target by checking if any object is in detection range.
            RaycastHit[] _hits = SearchRange(Params.MobTransform.position, Params.Manager.Data.DetectionRange, Params.Manager.Data.DetectionLayerMask);

            Transform _target = null;

            // Get the closest object within detection range.
            if (_hits.Length > 0) {
                float _minDistance = Params.Manager.Data.DetectionRange;
                foreach (RaycastHit hit in _hits) {
                    if (hit.distance <= _minDistance) {
                        _minDistance = hit.distance;
                        _target = hit.transform;
                    }
                }
            }
            
            // If there is no any valid target.
            if (_target == null && Params.Manager.GetState() != MobStates.Stunned) {
                //Params.Manager.OnIdle?.Invoke();
                Params.Manager.SetState(MobStates.Idle);
            } 

            // An object entered the detection range while the mob was not alert.
            if (Params.Manager.GetState() == MobStates.Idle && _target != null) {
                //Params.Manager.OnEnterRange?.Invoke();
            }

            // All objects have left the detection range while the mob was alert.
            if (Params.Manager.GetState() != MobStates.Idle && _target == null) {
                //Params.Manager.OnExitRange?.Invoke();
            }

            // If there is a valid target, move to the target position. 
            if (_target != null) {
                
                if (Params.Manager.GetState() == MobStates.Attacking || Params.Manager.GetState() == MobStates.Stunned) return;

                // Attack
                if (Vector3.Distance(_target.position, Params.MobTransform.position) <= Params.Manager.Data.AttackRange) {
                    Params.Manager.OnAttack?.Invoke(new MobOnAttackParams(Params.Manager, _target, Params.Manager.Data.DamageDealt, 
                        Params.Manager.Data.AttackRange, Params.Manager.Data.AttackTime, Params.Manager.Data.DetectionLayerMask, 
                        Params.MobTransform, Params.Manager.Data.AttackAreaSize));
                    MoveToPosition(Params.NavAgent, Params.MobTransform.position);
                    Params.NavAgent.isStopped = true;
                    //Params.Manager.OnAnimation("Attacking");
                }
                // Follow
                else {
                    Params.Manager.SetState(MobStates.Following);
                    //Params.Manager.OnAnimation("Moving");
                    Vector3 _targetPosition;
                    _targetPosition = _target.position;
                    MoveToPosition(Params.NavAgent, _target.position);
                }
            }
            else {
                MoveToPosition(Params.NavAgent, Params.MobTransform.position);
                //if (!Params.NavAgent.isStopped) Params.NavAgent.isStopped = true;
                //Params.Manager.OnAnimation("Idle");
            }

            // When the mob is stunned.
            if (Params.Manager.GetState() == MobStates.Stunned) {
                if (!Params.NavAgent.isStopped) {
                    Params.NavAgent.velocity = Vector3.zero;
                    Params.NavAgent.isStopped = true;
                }
            }
            else {
                Params.NavAgent.isStopped = false;
            }
    }

    public virtual void OnAttack(MobOnAttackParams Params) {
        Params.Manager.StartCoroutine(HandleAttackingState(Params));
    }

    public virtual void HandleHit(MobOnHitParams Params) {
        if (Params.Manager.IsInvulnerable) return;
        Params.Manager.OnStunned?.Invoke(Params.Manager, Params.StunDuration);
        HandleKnockback(Params.Rb, Params.Knockback, Params.Direction);
        HandleDamage(Params.Manager, Params.DamageTaken);
    }

    public virtual void HandleKnockback(Rigidbody Rb, float Knockback, Vector3 Direction) {
        // Custom knockback calculations can be added here to determine a final knockback amount.
        if (Knockback == 0) return;
        Rb.AddForce(new Vector3(Direction.x, 2f, Direction.z) * Knockback, ForceMode.Impulse);
    }

    public virtual void HandleDamage(MobManager Manager, float DamageTaken) {
        // Custom damage calculations can be added here to determine a final damage taken.
        if (DamageTaken <= 0f) return;
        Manager.OnDamageTaken?.Invoke(Manager, DamageTaken);
    }

    public  virtual void HandleDeath(MobManager Manager) {
        //Manager.OnAnimation("Dying");
        Manager.GetComponent<PoolableObjectController>().EnqueueCheck(Manager.Data.PoolingTime);
        WaveManager.Instance.RemoveFromSpawnedList(Manager.gameObject);
        MobSpawnerManager.Instance.SpawnOrbWithPooling(Manager.gameObject.transform.position);
        Manager.StopAllCoroutines();
        Manager.SetState(MobStates.StopAI);
        Manager.Clear();
        FindObjectOfType<theArrowMovement>().RemoveFromTaggedEnemyList(Manager.transform.GetChild(1).transform);
    }

    #region Event Functions

    public virtual void OnStunned(MobManager Manager, float Duration) {
        if (Duration <= 0f) return;
        Manager.StartCoroutine(HandleStunnedState(Manager, Duration));
    }
    public virtual void OnAttackHit(MobOnAttackHitParams Params) { 
        foreach (Collider collider in Params.Target) {
            collider.GetComponent<PlayerHealthManager>().TakeDamage(Params.DamageDealt);
        }
    }

    public virtual void OnHit(MobOnHitParams Params) { 
        HandleHit(Params); 
    }

    public virtual void OnDamageTaken(MobManager Manager, float DamageTaken) { 
        Manager.CurrentHealth -= DamageTaken;
        if (Manager.CurrentHealth <= 0f) Manager.OnDeath?.Invoke(Manager);
        else Manager.StartCoroutine(HandleInvulnerableState(Manager));
    }

    public virtual void OnDeath(MobManager Manager) { 
        HandleDeath(Manager);
    }

    public virtual void OnEnterRange(MobManager Manager) {
        
    }

    public virtual void OnExitRange(MobManager Manager) {
        
    }

    #endregion

    #endregion

    #region Non-Virtual Functions

    public void FollowPath() {
        // DO I NEED THIS? DISCUSS WITH THE TEAM MEMBERS.
    }

    public void SpawnAttackIndicator() {

    }

    public void MoveToPosition(NavMeshAgent Agent, Vector3 Position) { 
        Agent.SetDestination(Position);
    }

    public RaycastHit[] SearchRange(Vector3 CenterPosition, float DetectionRange, LayerMask Mask) {       
        RaycastHit[] hits = Physics.SphereCastAll(CenterPosition, DetectionRange, Vector3.up, DetectionRange, Mask);
        return hits;
    }

    public IEnumerator HandleStunnedState(MobManager Manager, float Duration) {
        if (Manager.GetState() == MobStates.Stunned) yield break;

        Manager.SetState(MobStates.Stunned);
        float elapsedTime = 0;
        while(elapsedTime < Duration) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (Manager.GetState() == MobStates.Stunned)
        Manager.SetState(MobStates.Idle);
    }

    public IEnumerator HandleAttackingState(MobOnAttackParams Params) {
        if (Params.Manager.GetState() == MobStates.Attacking) yield break;

        Params.Manager.SetState(MobStates.Attacking);

        // Spawn the field towards the player's direction.
        Vector3 direction = (Params.MobTransform.position - Params.Target.position).normalized;
        //Params.MobTransform.forward = direction;
        Params.Manager.SpawnAttackIndicator?.Invoke();

        float elapsedTime = 0;
        while(elapsedTime < Params.AttackTime) {
            
            // If the mob gets stunned while charging an attack, cancel the attack.
            if (Params.Manager.GetState() == MobStates.Stunned) yield break;

            elapsedTime += Time.deltaTime;
            // Add attack area fill animation to here.
            Params.Manager.UpdateAttackIndicator?.Invoke(elapsedTime / Params.AttackTime);
            yield return null;
        }

        Params.Manager.DespawnAttackIndicator?.Invoke();
        // If target is still within the attack area, deal damage to them.
        Collider[] colliders = Physics.OverlapBox(Params.MobTransform.position + Params.MobTransform.forward *  Params.AttackAreaSize.z / 2 , Params.AttackAreaSize / 2, 
        Quaternion.LookRotation(Params.MobTransform.forward, Vector3.up), Params.Mask);
        if (colliders.Length > 0) {
            Params.Manager.OnAttackHit?.Invoke(new MobOnAttackHitParams(colliders, Params.DamageDealt));
        } 

        if (Params.Manager.GetState() == MobStates.Stunned) yield break;
        if (Params.Manager.GetState() == MobStates.Attacking) Params.Manager.SetState(MobStates.Idle);
    }

    public IEnumerator HandleInvulnerableState(MobManager Manager) {
        Manager.IsInvulnerable = true;
        yield return new WaitForSecondsRealtime(Manager.Data.InvulnerableTime);
        Manager.IsInvulnerable = false;
    }

    #endregion
}
