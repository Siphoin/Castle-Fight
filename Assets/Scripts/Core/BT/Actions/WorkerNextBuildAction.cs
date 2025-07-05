using CastleFight.Core.UnitsSystem.Handlers;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WorkerNextBuild", story: "[Worker] Next Build", category: "Worker", id: "2d843ad44cdd6d6f4c377e1a68ee21a7")]
public partial class WorkerNextBuildAction : Action
{
    [SerializeReference] public BlackboardVariable<WorkerControlHandler> Worker;

    protected override Status OnStart()
    {
        Worker.Value.MoveToNextBuild();
        return Status.Success;
    }
}

