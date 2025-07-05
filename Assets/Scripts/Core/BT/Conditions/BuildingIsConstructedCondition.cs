using CastleFight.Core.BuildingsSystem;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "BuildingIsConstructed", story: "[Building] is Constructed", category: "Building", id: "f2a125f1f224c2ec18f76bce52fbc2d1")]
public partial class BuildingIsConstructedCondition : Condition
{
    [SerializeReference] public BlackboardVariable<BuildingInstance> Building;

    public override bool IsTrue()
    {
        return Building.Value is null ? true : Building.Value.IsContructed;
    }
}
