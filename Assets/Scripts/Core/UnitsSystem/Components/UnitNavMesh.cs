using System.Collections;
using CastleFight.Core.AI;
using CastleFight.Core.BuildingsSystem;
using CastleFight.Core.HealthSystem;
using Sirenix.OdinInspector;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace CastleFight.Core.UnitsSystem.Components
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent (typeof(BehaviorGraphAgent))]
    public class UnitNavMesh : MonoBehaviour, IUnitNavMesh
    {
        [SerializeField, ReadOnly] private UnitInstance _unitInstance;
        [SerializeField, ReadOnly] private NavMeshAgent _agent;
        [SerializeField, ReadOnly] private BehaviorGraphAgent _agentGraph;

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

        private void Start()
        {
            // test

            SpeedMovement = _agent.speed;

        }

        public void SetTarget(HealthComponent healthComponent)
        {
            CurrentTarget = healthComponent;
            CurrentState = UnitStateType.MoveToTarget;
        }

        public void SetIdle ()
        {
            CurrentTarget = null;
            CurrentState = UnitStateType.Idle;
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
    }
}