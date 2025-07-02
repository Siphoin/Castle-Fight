using CastleFight.Core.UnitsSystem;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using CastleFight.Core;
using CastleFight.Core.HealthSystem;
using CastleFight.Core.BuildingsSystem;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Checking Enemies in Radius", story: "Check Enemies in radius [Radius] for [Unit]", category: "Unit", id: "06db7b4d112a229c9d3c7ab9a6112f90")]
public partial class CheckingEnemiesInRadiusAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Radius;
    [SerializeReference] public BlackboardVariable<UnitInstance> Unit;

    protected override Status OnUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(Unit.Value.transform.position, Radius.Value);
        Debug.Log("checking");
        foreach (Collider collider in hitColliders)
        {
            if (collider.TryGetComponent(out UnitInstance otherUnit))
            {
                if (otherUnit.IsEnemy(Unit.Value) && otherUnit.HealthComponent != Unit.Value.NavMesh.CurrentTarget)
                {
                    Unit.Value.NavMesh.SetTarget(otherUnit.HealthComponent as HealthComponent);
                    Debug.Log("enemy");
                    return Status.Success;
                }
            }

            if (collider.TryGetComponent(out BuildingInstance otherBuilding))
            {
                if (otherBuilding.IsEnemy(Unit.Value) && otherBuilding.HealthComponent != Unit.Value.NavMesh.CurrentTarget)
                {
                    Unit.Value.NavMesh.SetTarget(otherUnit.HealthComponent as HealthComponent);
                    return Status.Success;
                }
            }
        }
        return Status.Running;
    }
}

