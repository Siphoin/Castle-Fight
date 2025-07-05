using CastleFight.Core.BuildingsSystem;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "BuildingIsNull", story: "[Building] Is Null", category: "Building", id: "58940ad595bc260bf541d684b53107c4")]
public partial class BuildingIsNullCondition : Condition
{
    [SerializeReference] public BlackboardVariable<BuildingInstance> Building;

    public override bool IsTrue()
    {
        return Building.Value is null;
    }
}
