using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Managers;
using Assets.Scripts.Enums;
using Assets.Scripts.Params;

namespace Assets.Scripts.Controllers {
    public class MobMovementController : MonoBehaviour
    {
        private MobManager _manager;
        private NavMeshAgent _navMeshAgent;
        
        private void OnEnable() {
            _manager.OnSetup += OnSetup;
        }

        private void OnDisable() {
            _manager.OnSetup -= OnSetup;
        }

        private void Awake() {
            _manager = GetComponent<MobManager>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void OnSetup() {
            _navMeshAgent.stoppingDistance = _manager.Data.StoppingDistance;
            _navMeshAgent.speed = _manager.Data.FollowSpeed;
        }

        private void Update() {
            if (_manager.GetState() == MobStates.NoData) return;

            _manager.OnMove?.Invoke(new MobOnMoveParams(_manager, _navMeshAgent, transform));
        }      
    }
}

