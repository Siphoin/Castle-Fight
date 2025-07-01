using CastleFight.Core.HealthSystem;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Health Object is Dead", story: "[Target] is Dead", category: "Health", id: "a29d0beb235df90dba9749b3ecbbaadc")]
public partial class HealthObjectIsDeadCondition : Condition
{
    [SerializeReference] public BlackboardVariable<HealthComponent> Target;

    public override bool IsTrue()
    {
        if (!Target.Value)
        {
            return true;
        }
        return Target.Value.IsDead;
    }
}
