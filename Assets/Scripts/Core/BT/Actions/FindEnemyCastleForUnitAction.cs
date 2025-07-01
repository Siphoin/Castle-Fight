using CastleFight.Core.UnitsSystem;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using CastleFight.Core.BuildingsSystem;
using CastleFight.Core.HealthSystem;
using ObjectRepositories.Extensions;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Find Enemy Castle For Unit", story: "Find enemy Castle for [Unit]", category: "Action", id: "9d8570b0a2a5aba18a9731ffff0d8848")]
public partial class FindEnemyCastleForUnitAction : Action
{
    [SerializeReference] public BlackboardVariable<UnitInstance> Unit;

    protected override Status OnStart()
    {
        var enemyCastle = Unit.Value.FindByConditionOnRepository<BuildingInstance>(x => x.IsEnemy(Unit.Value));

        if (enemyCastle is null)
        {
            return Status.Running;
        }

        Unit.Value.NavMesh.SetTarget(enemyCastle.HealthComponent as HealthComponent);

        return Status.Success;
    }
}

