using System.Collections;
using CastleFight.Core.AI;
using UnityEngine;

namespace CastleFight.Core.UnitsSystem.Components
{
    public class WorkerNavMesh : UnitNavMesh
    {
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
            CurrentStateWorker = WorkerUnitStateType.MoveToPoint;
        }
    }
}