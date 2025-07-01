using CastleFight.Core.UnitsSystem;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using CastleFight.Core.HealthSystem;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Unit Move To Current Target", story: "[Unit] Move To Current Target", category: "Unit", id: "983a2a2b4f072af8105d87d446dce404")]
public partial class UnitMoveToCurrentTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<UnitInstance> Unit;


    protected override Status OnUpdate()
    {
        Unit.Value.NavMesh.Move();
        return Status.Success;
    }
}

