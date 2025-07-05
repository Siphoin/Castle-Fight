using CastleFight.Core.UnitsSystem.Handlers;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WorkerCancelTasksBuilds", story: "[Worker] Cancel Tasks Builds", category: "Worker", id: "9f7f6a83fe71c1be94c3ea5ccfba330e")]
public partial class WorkerCancelTasksBuildsAction : Action
{
    [SerializeReference] public BlackboardVariable<WorkerControlHandler> Worker;

    protected override Status OnStart()
    {
        Worker.Value.CancelAllBuildTasks();
        return Status.Running;
    }
}

