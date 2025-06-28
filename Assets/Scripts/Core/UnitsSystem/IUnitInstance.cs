using CastleFight.Core.UnitsSystem.Components;
using CastleFight.Main.Factories;

namespace CastleFight.Core.UnitsSystem
{
    public interface IUnitInstance : IFactoryObject, ILivingEntity, IOwnerable
    {
        IUnitNavMesh NavMesh { get; }
        IUnitAnimatorHandler AnimatorHandler { get; }
    }
}