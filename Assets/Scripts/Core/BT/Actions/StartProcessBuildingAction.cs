using CastleFight.Core.BuildingsSystem;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StartProcessBuilding", story: "Start Building [Building]", category: "Building", id: "02b365d4539a52377d7a986a4325b1ce")]
public partial class StartProcessBuildingAction : Action
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
        Building.Value.ConstructHandler.ConstructProcess = true;
        return Status.Success;
    }

}

