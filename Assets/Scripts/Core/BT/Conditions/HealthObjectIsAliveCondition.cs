using CastleFight.Core.HealthSystem;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Health Object is Alive", story: "[Target] is Alive", category: "Health", id: "3bbaad853bed71304b29f5b2c070f257")]
public partial class HealthObjectIsAliveCondition : Condition
{
    [SerializeReference] public BlackboardVariable<HealthComponent> Target;

    public override bool IsTrue()
    {
        if (!Target.Value)
        {
            return false;
        }
        return Target.Value.IsDead == false;
    }
}
