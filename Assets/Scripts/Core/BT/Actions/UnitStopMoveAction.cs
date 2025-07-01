using CastleFight.Core.UnitsSystem;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Unit Stop Move", story: "[Unit] Stop Move", category: "Unit", id: "350ffbb55399d582fa64027fab78a792")]
public partial class UnitStopMoveAction : Action
{
    [SerializeReference] public BlackboardVariable<UnitInstance> Unit;

    protected override Status OnStart()
    {
        Unit.Value.NavMesh.Stop();
        return Status.Success;
    }
}

