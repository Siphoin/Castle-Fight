using CastleFight.Core.AI;
using CastleFight.Core.BuildingsSystem;
using CastleFight.Core.HealthSystem;
using CastleFight.Core.UnitsSystem.Configs;
using ObjectRepositories.Extensions;
using Sirenix.OdinInspector;
using UniRx;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
namespace CastleFight.Core.UnitsSystem.Components
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent (typeof(BehaviorGraphAgent))]
    public class UnitNavMesh : MonoBehaviour, IUnitNavMesh
    {
        [SerializeField, ReadOnly] private UnitInstance _unitInstance;
        [SerializeField, ReadOnly] private NavMeshAgent _agent;
        [SerializeField, ReadOnly] private BehaviorGraphAgent _agentGraph;
        [Inject] private UnitGlobalConfig _unitGlobalConfig;

        private const float ROTATION_SPEED_DEG_PER_SECONDS = 180f;

        public IHealthComponent CurrentTarget
        {
            get
            {
                return _agentGraph.BlackboardReference.Blackboard.Variables[1].ObjectValue as IHealthComponent;
            }

            private set
            {
                _agentGraph.BlackboardReference.Blackboard.Variables[1].ObjectValue = value as HealthComponent;
            }
        }

        public UnitStateType CurrentState
        {
            get
            {
                return (UnitStateType)_agentGraph.BlackboardReference.Blackboard.Variables[2].ObjectValue;
            }

            private set
            {
                _agentGraph.BlackboardReference.Blackboard.Variables[2].ObjectValue = value;
            }
        }

        public float SpeedMovement
        {
            get
            {
                return (float)_agentGraph.BlackboardReference.Blackboard.Variables[4].ObjectValue;
            }

            private set
            {
                _agentGraph.BlackboardReference.Blackboard.Variables[4].ObjectValue = value;
            }
        }

        public float SpeedAttack
        {
            get
            {
                return (float)_agentGraph.BlackboardReference.Blackboard.Variables[7].ObjectValue;
            }

            private set
            {
                _agentGraph.BlackboardReference.Blackboard.Variables[7].ObjectValue = value;
            }
        }

        private void Start()
        {
            _agent.updateRotation = false;
            enabled = _unitInstance.IsOwner;

            if (_unitInstance.IsOwner)
            {
                SetNavMeshParameters();
                _unitInstance.HealthComponent.OnCurrentHealthChanged.Subscribe(health =>
                {
                    if (health <= 0)
                    {
                        SetDeath();
                    }

                }).AddTo(this);
            }
        }

        private void Update()
        {
            if (CurrentTarget != null)
            {
                Vector3 direction = _agent.steeringTarget - transform.position;
                direction.y = 0;

                if (direction.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    transform.rotation = RotateTowardsLimited(transform.rotation, targetRotation, _unitGlobalConfig.RotationSpeed * Time.deltaTime);
                }
            }
        }

        private Quaternion RotateTowardsLimited(Quaternion current, Quaternion target, float maxDegreesDelta)
        {
            return Quaternion.RotateTowards(current, target, maxDegreesDelta);
        }





        private void SetNavMeshParameters()
        {
            _agent.speed = _unitInstance.Stats.MoveSpeed;
            SpeedMovement = _agent.speed;
            SpeedAttack = _unitInstance.Stats.AttackSpeed;
        }

        public void SetTarget(HealthComponent healthComponent)
        {
            CurrentTarget = healthComponent;
            CurrentState = UnitStateType.MoveToTarget;
        }

        public void Move ()
        {
            _agent.isStopped = false;
            HealthComponent healthComponent = CurrentTarget as HealthComponent;
            _agent.SetDestination(healthComponent.transform.position);
        }

        public void Stop ()
        {
            _agent.isStopped = true;
        }

        public void SetIdle ()
        {
            CurrentTarget = null;
            CurrentState = UnitStateType.Idle;
        }

        private void SetDeath ()
        {
            CurrentTarget = null;
            CurrentState = UnitStateType.Dead;
        }

        private void OnValidate()
        {
            if (!_agent)
            {
                _agent = GetComponent<NavMeshAgent>();
            }

            if (!_unitInstance)
            {
                _unitInstance = GetComponent<UnitInstance>();
            }

            if (!_agentGraph)
            {
                _agentGraph = GetComponent<BehaviorGraphAgent>();
            }
        }

        public void Disable()
        {
            CurrentTarget = null;
            CurrentState = UnitStateType.Dead;
            _agent.isStopped = true;
            _agentGraph.enabled = false;
        }
    }
}