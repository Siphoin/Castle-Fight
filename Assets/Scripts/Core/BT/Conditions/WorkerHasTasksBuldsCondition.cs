using CastleFight.Core.UnitsSystem.Handlers;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "WorkerHasTasksBulds", story: "[Worker] Has Tasks Builds", category: "Worker", id: "ff88c15d360d5a92a7305bbcc5f00ee0")]
public partial class WorkerHasTasksBuldsCondition : Condition
{
    [SerializeReference] public BlackboardVariable<WorkerControlHandler> Worker;

    public override bool IsTrue()
    {
        return Worker.Value.HasTasksBuild();
    }
}
