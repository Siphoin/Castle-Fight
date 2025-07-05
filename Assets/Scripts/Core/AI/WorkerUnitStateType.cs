using System;
using Unity.Behavior;
namespace CastleFight.Core.AI
{
    [BlackboardEnum]
    public enum WorkerUnitStateType
    {
        Idle,
        MoveToPoint,
        Constructing,
        MoveToBuild,
        WaitSelectPointBuild,
    }
}
