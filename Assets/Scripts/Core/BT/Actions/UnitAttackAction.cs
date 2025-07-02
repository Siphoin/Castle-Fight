using CastleFight.Core.UnitsSystem;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Unit Attack", story: "[Unit] Attack", category: "Unit", id: "60e14f5cc0e3c0c90ff7ad5f36e8de66")]
public partial class UnitAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<UnitInstance> Unit;

    protected override Status OnStart()
    {
        Unit.Value.AnimatorHandler.PlayAnimationAttack();
        return Status.Running;
    }
}

