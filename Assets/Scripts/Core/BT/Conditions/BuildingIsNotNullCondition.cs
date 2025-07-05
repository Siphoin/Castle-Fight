using CastleFight.Core.BuildingsSystem;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "BuildingIsNotNull", story: "[Building] Is Not Null", category: "Building", id: "884eae312069c2b2a7a5348c081924d6")]
public partial class BuildingIsNotNullCondition : Condition
{
    [SerializeReference] public BlackboardVariable<BuildingInstance> Building;

    public override bool IsTrue()
    {
        return Building.Value != null;
    }
}
