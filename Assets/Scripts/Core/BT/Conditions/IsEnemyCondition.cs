using System;
using CastleFight.Core;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is Enemy", story: "[Target] is Enemy for [Agent]", category: "Teamable Object", id: "e3a034374d9f280a9cde81c0eb50cf70")]
public partial class IsEnemyCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    public override bool IsTrue()
    {
        if (Agent.Value.TryGetComponent(out ITeamableObject agentTeam))
        {
            if (Target.Value.TryGetComponent(out IOwnerable targetTeam))
            {
                return agentTeam.IsEnemy(targetTeam);
            }
        }
        return false;
    }
}
