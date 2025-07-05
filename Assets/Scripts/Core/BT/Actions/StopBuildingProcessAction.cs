using CastleFight.Core.BuildingsSystem;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StopBuildingProcess", story: "Stop Building [Building]", category: "Building", id: "bae0f3bc14e39c2a2bef99c6473f4418")]
public partial class StopBuildingProcessAction : Action
{
    [SerializeReference] public BlackboardVariable<BuildingInstance> Building;

    protected override Status OnStart()
    {
        if (Building.Value is null)
        {
            return Status.Failure;
        }

        if (Building.Value.HealthComponent.IsDead)
        {
            return Status.Failure;
        }
        Building.Value.ConstructHandler.ConstructProcess = false;
        return Status.Success;
    }
}

