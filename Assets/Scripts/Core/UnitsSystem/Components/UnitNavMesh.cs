﻿using CastleFight.Core.AI;
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

        public virtual float SpeedMovement
        {
            get
            {
                return (float)_agentGraph.BlackboardReference.Blackboard.Variables[4].ObjectValue;
            }

            protected set
            {
                _agentGraph.BlackboardReference.Blackboard.Variables[4].ObjectValue = value;
            }
        }

        public virtual float SpeedAttack
        {
            get
            {
                return (float)_agentGraph.BlackboardReference.Blackboard.Variables[7].ObjectValue;
            }

            protected set
            {
                _agentGraph.BlackboardReference.Blackboard.Variables[7].ObjectValue = value;
            }
        }

        protected BehaviorGraphAgent AgentGraph => _agentGraph;

        protected UnitGlobalConfig UnitGlobalConfig => _unitGlobalConfig;

        protected NavMeshAgent Agent => _agent;

        protected virtual void Start()
        {
            _agent.updateRotation = false;

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

        protected virtual void Update()
        {
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = Agent.steeringTarget;

            Vector3 moveDirection = targetPosition - currentPosition;
            moveDirection.y = 0;

            float distanceSqr = moveDirection.sqrMagnitude;
            float stopThreshold = 0.01f;    // порог для "движения"
            float closeThreshold = 0.1f;    // порог близости к цели

            if (distanceSqr > stopThreshold)
            {
                // Юнит движется - смотрит по движению
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);
            }
            else
            {
                // Юнит стоит или близко к цели — поворачиваем к цели
                Vector3 directionToTarget = targetPosition - currentPosition;
                directionToTarget.y = 0;

                if (directionToTarget.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget.normalized);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);
                }
            }
        }

        protected Quaternion RotateTowardsLimited(Quaternion current, Quaternion target, float maxDegreesDelta)
        {
            return Quaternion.RotateTowards(current, target, maxDegreesDelta);
        }





        private void SetNavMeshParameters()
        {
            _agent.speed = _unitInstance.Stats.MoveSpeed;
            try
            {
                SpeedMovement = _agent.speed;
            }
            catch
            {
            }
            try
            {
                SpeedAttack = _unitInstance.Stats.AttackSpeed;
            }
            catch
            {

            }
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

        public virtual void SetIdle ()
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