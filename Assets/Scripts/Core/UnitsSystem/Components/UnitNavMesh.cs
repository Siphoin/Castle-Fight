using System.Collections;
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

        private void Start()
        {
            // test

            if (_unitInstance.IsMy)
            {
                var building = FindAnyObjectByType<BuildingInstance>();

                SetTarget(building.HealthComponent as HealthComponent);
            }
        }

        public void SetTarget(HealthComponent healthComponent)
        {
            CurrentTarget = healthComponent;
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