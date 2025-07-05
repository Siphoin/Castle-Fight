using CastleFight.Core.UnitsSystem.Handlers;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "WorkerNotHasTasksBuilds", story: "[Worker] Not Has Tasks Builds", category: "Worker", id: "6dd8582e7ad9e029d42b021c175d9937")]
public partial class WorkerNotHasTasksBuildsCondition : Condition
{
    [SerializeReference] public BlackboardVariable<WorkerControlHandler> Worker;

    public override bool IsTrue()
    {
        return Worker.Value.HasTasksBuild() == false;
    }
}
