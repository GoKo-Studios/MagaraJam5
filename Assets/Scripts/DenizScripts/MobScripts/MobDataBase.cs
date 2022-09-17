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
            Vector3 _targetPosition = Vector3.zero;

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
                
                Vector3 _directionTowardsPlayer = (_target.position - Params.MobTransform.position).normalized;
                Vector3 _directionTowardsTarget = (Params.MobTransform.position - _target.position).normalized;

                if (Params.Manager.GetState() == MobStates.Attacking || Params.Manager.GetState() == MobStates.Stunned) return;
            
                // Attack
                if (Vector3.Distance(_target.position, Params.MobTransform.position) <= Params.Manager.Data.AttackRange) {
                    
                    if (Vector3.Angle(Params.MobTransform.forward, _directionTowardsPlayer) <= Params.Manager.Data.AttackAngleTreshold) {
                        Params.Manager.OnAttack?.Invoke(new MobOnAttackParams(Params.Manager, _target, Params.Manager.Data.DamageDealt, 
                            Params.Manager.Data.AttackRange, Params.Manager.Data.AttackTime, Params.Manager.Data.DetectionLayerMask, 
                            Params.MobTransform, Params.Manager.Data.AttackAreaSize));

                        _targetPosition = Params.MobTransform.position;
                        Params.NavAgent.isStopped = true;
                        Params.Manager.OnAnimation("Attack", MobAnimationControllerTypes.Trigger, true);
                    }
                }
                // Follow
                else {
                    Params.Manager.SetState(MobStates.Following);
                    Params.Manager.OnAnimation("Walk", MobAnimationControllerTypes.Trigger, true);     
                    _targetPosition = _target.position;
                    
                }
            }
            else {
                // If there is no any valid target.
                if (Params.Manager.GetState() != MobStates.Panic && Params.Manager.GetState() != MobStates.Stunned && Params.Manager.GetState() != MobStates.Attacking) {
                    _targetPosition = Params.MobTransform.position;
                    //Params.Manager.OnIdle?.Invoke();
                    Params.Manager.SetState(MobStates.Idle);
                    //if (!Params.NavAgent.isStopped) Params.NavAgent.isStopped = true;
                    Params.Manager.OnAnimation("Idle", MobAnimationControllerTypes.Trigger, true);
                }     
            }

            // When the mob is stunned.
            if (Params.Manager.GetState() == MobStates.Stunned) {
                if (!Params.NavAgent.isStopped) {
                    _targetPosition = Params.MobTransform.position;
                    Params.NavAgent.velocity = Vector3.zero;
                    Params.NavAgent.isStopped = true;
                }
            }
            else {
                Params.NavAgent.isStopped = false;
            }

            // When the mob is panicking.
            if (Params.Manager.GetState() == MobStates.Panic) {
                float randomPosX = Random.Range(-Params.Manager.Data.PanicRandomRange, Params.Manager.Data.PanicRandomRange + 1);
                float randomPosZ = Random.Range(-Params.Manager.Data.PanicRandomRange, Params.Manager.Data.PanicRandomRange + 1);
                _targetPosition = new Vector3(Params.MobTransform.position.x + randomPosX, 0, Params.MobTransform.position.z + randomPosZ);
            }

            MoveToPosition(Params.NavAgent, _targetPosition);
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
        Manager.OnAnimation("Death", MobAnimationControllerTypes.Trigger, true);
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
        DeathPanic(Manager, Manager.transform);
        HandleDeath(Manager);
    }

    public void OnPanic(MobManager Manager) {
        if (Manager.GetState() != MobStates.Idle) return;
        Manager.StartCoroutine(HandlePanicState(Manager, Manager.Data.PanicDuration));
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

    public void MoveToPosition(NavMeshAgent Agent, Vector3 Position) { 
        Agent.SetDestination(Position);
    }

    public RaycastHit[] SearchRange(Vector3 CenterPosition, float DetectionRange, LayerMask Mask) {       
        RaycastHit[] hits = Physics.SphereCastAll(CenterPosition, DetectionRange, Vector3.up, DetectionRange, Mask);
        return hits;
    }

    public void DeathPanic(MobManager Manager, Transform MobTransform) {
        Collider[] colliders = Physics.OverlapSphere(MobTransform.position, Manager.Data.OnDeathPanicRange,Manager.Data.PanicLayerMask);
        foreach (Collider col in colliders) {
            MobManager manager = col.GetComponentInParent<MobManager>();
            manager.OnPanic?.Invoke(manager);
        }
    }

    public void LookAtTarget(Transform MobTransform, Vector3 Direction) {
        Direction.y = 0f;
        MobTransform.forward = Direction;
    }

    public IEnumerator HandleStunnedState(MobManager Manager, float Duration) {
        //if (Manager.GetState() == MobStates.Stunned) yield break;
        Debug.Log(Duration);
        Manager.SetState(MobStates.Stunned);
        float elapsedTime = 0;
        while(elapsedTime < Duration) {
            if(Manager.GetState() != MobStates.Stunned){
                
            }
            Debug.Log(elapsedTime);
            Manager.SetState(MobStates.Stunned);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (Manager.GetState() == MobStates.Stunned)
        Manager.SetState(MobStates.Idle);
    }

    public virtual IEnumerator HandleAttackingState(MobOnAttackParams Params) {
        if (Params.Manager.GetState() == MobStates.Attacking) yield break;

        Params.Manager.SetState(MobStates.Attacking);

        // Spawn the field towards the player's direction.
        Vector3 direction = (Params.MobTransform.position - Params.Target.position).normalized;
        //Params.MobTransform.forward = direction;
        Params.Manager.SpawnAttackIndicator?.Invoke();

        float elapsedTime = 0;
        while(elapsedTime < Params.AttackTime) {
            
            // If the mob gets stunned while charging an attack, cancel the attack.

            if (Params.Manager.GetState() == MobStates.Stunned) {
                Params.Manager.DespawnAttackIndicator?.Invoke();
                yield break;
            } 

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

    public IEnumerator HandlePanicState(MobManager Manager, float Duration) {
        Manager.SetState(MobStates.Panic);

        float elapsedTime = 0;
        while(elapsedTime < Duration) {
            elapsedTime += Time.deltaTime;
            Debug.Log("Looping");
            if (Manager.GetState() == MobStates.Stunned) yield break;
            if (Manager.GetState() == MobStates.Attacking) yield break;
            if (Manager.GetState() == MobStates.Following) yield break;
            yield return null;
        }

        if (Manager.GetState() == MobStates.Stunned) yield break;
        if (Manager.GetState() == MobStates.Attacking) yield break;
        Manager.SetState(MobStates.Idle);
    }

    #endregion
}
