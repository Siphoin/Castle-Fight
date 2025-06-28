using System;
using Unity.Behavior;
namespace CastleFight.Core.AI
{
    [BlackboardEnum]
    public enum UnitStateType
    {
        Idle,
        MoveToTarget,
        MeleeAttack
    }
}
