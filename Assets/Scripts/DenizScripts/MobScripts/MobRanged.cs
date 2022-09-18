using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Params;
using UnityEngine;
using Assets.Scripts.Enums;
using Assets.Scripts.Managers;


[CreateAssetMenu(menuName = "ScriptableObjects/MobData/Ranged", fileName = "NewRangedMob")]
public class MobRanged : MobDataBase
{
    public float DistanceToKeep;
    public float DistanceToFollow;
    public LayerMask RangedCollisionLayerMask;

    public override void MovementController(MobOnMoveParams Params)
    {

         if (Params.Manager.GetState() == MobStates.StopAI) return;
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
        
            Vector3 _directionTowardsPlayer = (_target.position - Params.MobTransform.position).normalized;
            Vector3 _directionTowardsTarget = (Params.MobTransform.position - _target.position).normalized;
            float _distance = Vector3.Distance(Params.MobTransform.position, _target.position);

            if (_distance >= DistanceToFollow) {
                // Follow

                Params.Manager.SetState(MobStates.Following);
                 Params.Manager.OnAnimation("Run", MobAnimationControllerTypes.Bool, true); 
                Params.Manager.OnAnimation("Attack", MobAnimationControllerTypes.Bool, false); 
                
                Vector3 _targetPosition = _target.position;
                MoveToPosition(Params.NavAgent, _targetPosition);

            }
            else if (_distance >= DistanceToKeep && _distance <= Params.Manager.Data.AttackRange) {
                // Attack

                LookAtTarget(Params.MobTransform, _directionTowardsPlayer);
                if (!Physics.Raycast((Params.MobTransform.position + Params.MobTransform.forward * 1f), _directionTowardsPlayer,
                         Params.Manager.Data.AttackRange, RangedCollisionLayerMask)) {

                    if (Vector3.Angle(Params.MobTransform.forward, _directionTowardsPlayer) <= Params.Manager.Data.AttackAngleTreshold) {      
                        Params.Manager.OnAttack?.Invoke(new MobOnAttackParams(Params.Manager, _target, Params.Manager.Data.DamageDealt, 
                            Params.Manager.Data.AttackRange, Params.Manager.Data.AttackTime, Params.Manager.Data.DetectionLayerMask, 
                                Params.MobTransform, Params.Manager.Data.AttackAreaSize));

                        MoveToPosition(Params.NavAgent, Params.MobTransform.position);
                        Params.NavAgent.isStopped = true;   
                        Params.Manager.OnAnimation("Run", MobAnimationControllerTypes.Bool, false); 
                         Params.Manager.OnAnimation("Attack", MobAnimationControllerTypes.Bool, true);   
                    }        
                }        
            }
            else if  (_distance <= DistanceToKeep) {
                // Keep Distance
                Debug.Log(_distance);
                Params.Manager.SetState(MobStates.Following);
                Params.Manager.OnAnimation("Run", MobAnimationControllerTypes.Bool, true); 
                Params.Manager.OnAnimation("Attack", MobAnimationControllerTypes.Bool, false); 
                
                float _distanceToTravel = DistanceToKeep - _distance;
                _directionTowardsTarget.y = 0;
                Vector3 _targetPosition = Params.MobTransform.position + _directionTowardsTarget * (_distanceToTravel); 
                MoveToPosition(Params.NavAgent, _targetPosition);
            }
            else {
                // Stop
                MoveToPosition(Params.NavAgent, Params.MobTransform.position);
                Params.Manager.OnAnimation("Run", MobAnimationControllerTypes.Bool, false); 
                Params.Manager.OnAnimation("Attack", MobAnimationControllerTypes.Bool, false); 
            }
        }
        else {
            // If there is no any valid target.

            if (Params.Manager.GetState() != MobStates.Panic && Params.Manager.GetState() != MobStates.Stunned && Params.Manager.GetState() != MobStates.Attacking) {
                MoveToPosition(Params.NavAgent, Params.MobTransform.position);
                //Params.Manager.OnIdle?.Invoke();
                Params.Manager.SetState(MobStates.Idle);
                //if (!Params.NavAgent.isStopped) Params.NavAgent.isStopped = true;
                Params.Manager.OnAnimation("Run", MobAnimationControllerTypes.Bool, false); 
                Params.Manager.OnAnimation("Attack", MobAnimationControllerTypes.Bool, false); 
            }
        }

        // When the mob is stunned.
        if (Params.Manager.GetState() == MobStates.Stunned) {
            if (!Params.NavAgent.isStopped) {
                MoveToPosition(Params.NavAgent, Params.MobTransform.position);
                Params.NavAgent.velocity = Vector3.zero;
                Params.NavAgent.isStopped = true;
                Params.Manager.OnAnimation("Run", MobAnimationControllerTypes.Bool, false); 
                Params.Manager.OnAnimation("Attack", MobAnimationControllerTypes.Bool, false); 
            }
        }
        else {
            Params.NavAgent.isStopped = false;
        }
    }

    public override IEnumerator HandleAttackingState(MobOnAttackParams Params)
    {
        if (Params.Manager.GetState() == MobStates.Attacking) yield break;

        Params.Manager.SetState(MobStates.Attacking);

        // Spawn the field towards the player's direction.
        Vector3 direction = (Params.Target.position - Params.MobTransform.position).normalized;
        Params.MobTransform.forward = direction;
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

        //Collider[] colliders = Physics.OverlapBox(Params.MobTransform.position + Params.MobTransform.forward *  Params.AttackAreaSize.z / 2 , Params.AttackAreaSize / 2, 
        //Quaternion.LookRotation(Params.MobTransform.forward, Vector3.up), Params.Mask);
        //if (colliders.Length > 0) {
        //    Params.Manager.OnAttackHit?.Invoke(new MobOnAttackHitParams(colliders, Params.DamageDealt));
        //} 

        MobSpawnerManager.Instance.SpawnBulletWithPooling(Params.MobTransform.position + Params.MobTransform.forward * 1f, direction);

        if (Params.Manager.GetState() == MobStates.Stunned) yield break;
        if (Params.Manager.GetState() == MobStates.Attacking) Params.Manager.SetState(MobStates.Idle);
    }
}
