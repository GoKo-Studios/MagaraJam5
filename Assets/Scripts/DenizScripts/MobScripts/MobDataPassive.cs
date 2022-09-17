using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Params;
using UnityEngine;
using Assets.Scripts.Enums;
using Assets.Scripts.Managers;

[CreateAssetMenu(menuName = "ScriptableObjects/MobData/Passive", fileName = "NewPassiveMob")]
public class MobDataPassive : MobDataBase
{
    public float DistanceToKeep;
    public float RandomRunDistance;

    public override void MovementController(MobOnMoveParams Params)
    {                   
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

        // If there is a valid target, run away from the target to keep distance. 
        if (_target != null) {
            float _distance = Vector3.Distance(Params.MobTransform.position, _target.position);
            if ( _distance < DistanceToKeep) {
                Params.Manager.SetState(MobStates.Following);
                Params.Manager.OnAnimation("Run", MobAnimationControllerTypes.Trigger, true);
                Vector3 _direction = (Params.MobTransform.position - _target.position).normalized;
                float _distanceToTravel = DistanceToKeep - _distance;
                Vector3 _targetPosition = Params.MobTransform.position + _direction * (_distanceToTravel); 
                MoveToPosition(Params.NavAgent, _targetPosition);
            }
            else {
                Params.Manager.OnAnimation("Idle", MobAnimationControllerTypes.Trigger, true); 
                MoveToPosition(Params.NavAgent, Params.MobTransform.position);
            }
        }
        else {
            // If there is no any valid target.
    
            MoveToPosition(Params.NavAgent, Params.MobTransform.position);
            //if (!Params.NavAgent.isStopped) Params.NavAgent.isStopped = true;
            //Params.Manager.OnIdle?.Invoke();
            Params.Manager.SetState(MobStates.Idle);
            Params.Manager.OnAnimation("Idle", MobAnimationControllerTypes.Trigger, true);
        }

        // When the mob is stunned.
        if (Params.Manager.GetState() == MobStates.Stunned) {
            if (!Params.NavAgent.isStopped) {
                MoveToPosition(Params.NavAgent, Params.MobTransform.position);
                Params.NavAgent.velocity = Vector3.zero;
                Params.NavAgent.isStopped = true;
            }
        }
        else {
            Params.NavAgent.isStopped = false;
        }
    }

    public override void OnEnterRange(MobManager Manager)
    {
        base.OnEnterRange(Manager);
    }

    public override void OnExitRange(MobManager Manager)
    {
        base.OnExitRange(Manager);
    }
}
