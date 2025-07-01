using CastleFight.Core.UnitsSystem;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Play Death Animation Unit", story: "Play Death Animation on unit [Unit]", category: "Unit", id: "4d2c4a6a154c772377f66691f7f32dd1")]
public partial class PlayDeathAnimationUnitAction : Action
{
    [SerializeReference] public BlackboardVariable<UnitInstance> Unit;

    protected override Status OnStart()
    {
        Unit.Value.AnimatorHandler.PlayAnimationDeath();
        return Status.Running;
    }
}

