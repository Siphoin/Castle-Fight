using CastleFight.Core.HealthSystem;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Target not seted for Unit", story: "[Target] not seted", category: "Unit", id: "56409921ec5e99102acc0728cecc1fae")]
public partial class TargetNotSetedForUnitCondition : Condition
{
    [SerializeReference] public BlackboardVariable<HealthComponent> Target;

    public override bool IsTrue()
    {
        return Target.Value is null;
    }
}
