using System.Collections;
using CastleFight.Core.AI;
using CastleFight.Core.BuildingsSystem;
using CastleFight.Core.ConstructionSystem.Events;
using CastleFight.Core.UnitsSystem.Configs;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace CastleFight.Core.UnitsSystem.Components
{
    public class WorkerNavMesh : UnitNavMesh
    {
        [SerializeField, ReadOnly] private UnitInstance _unit;
        public WorkerUnitStateType CurrentStateWorker
        {
            get
            {
                return (WorkerUnitStateType)AgentGraph.BlackboardReference.Blackboard.Variables[3].ObjectValue;
            }

            private set
            {
                AgentGraph.BlackboardReference.Blackboard.Variables[3].ObjectValue = value;
            }
        }

        public Vector3 MovePoint
        {
            get
            {
                return (Vector3)AgentGraph.BlackboardReference.Blackboard.Variables[4].ObjectValue;
            }

            private set
            {
                AgentGraph.BlackboardReference.Blackboard.Variables[4].ObjectValue = value;
            }
        }

        public override float SpeedMovement
        {
            get
            {
                return (float)AgentGraph.BlackboardReference.Blackboard.Variables[1].ObjectValue;
            }

            protected set
            {
                AgentGraph.BlackboardReference.Blackboard.Variables[1].ObjectValue = value;
            }
        }

        public BuildingInstance TargetBuilding
        {
            get
            {
                return (BuildingInstance)AgentGraph.BlackboardReference.Blackboard.Variables[5].ObjectValue;
            }

            private set
            {
                AgentGraph.BlackboardReference.Blackboard.Variables[5].ObjectValue = value;
            }
        }

        public override float SpeedAttack
        {
            get
            {
                return (float)AgentGraph.BlackboardReference.Blackboard.Variables[8].ObjectValue;
            }

            protected set
            {
                AgentGraph.BlackboardReference.Blackboard.Variables[8].ObjectValue = value;
            }
        }

        private void LateUpdate()
        {
            AgentGraph.enabled = _unit.IsOwner;
        }

        protected override void Start()
        {
            base.Start();
            SetIdle();
        }
        public override void SetIdle()
        {
           CurrentStateWorker = WorkerUnitStateType.Idle;
        }

        public void SetPointMove (Vector3 point)
        {
            CurrentStateWorker = WorkerUnitStateType.Idle;
            CurrentStateWorker = WorkerUnitStateType.MoveToPoint;
            MovePoint = point;
        }

        public void SetWaitSelectPointBuild ()
        {
            CurrentStateWorker = WorkerUnitStateType.WaitSelectPointBuild;
        }

        public void MoveToBuild (NewBuildingConstructEvent newBuildingConstructEvent)
        {
            TargetBuilding = newBuildingConstructEvent.Building as BuildingInstance;
            MovePoint = TargetBuilding.SpawnPoint;
            CurrentStateWorker = WorkerUnitStateType.MoveToBuild;
        }

        private void OnValidate()
        {
            if (!_unit) _unit = GetComponent<UnitInstance>();
        }



    }
}