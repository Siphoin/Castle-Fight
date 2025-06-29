using CastleFight.Core.SO;
using CastleFight.Core.UnitsSystem.Components;
using CastleFight.Core.UnitsSystem.SO;
using CastleFight.Main.Factories;

namespace CastleFight.Core.UnitsSystem
{
    public interface IUnitInstance : IFactoryObject, ILivingEntity, IOwnerable, IStatsableEntity<ScriptableUnitEntity>
    {
        IUnitNavMesh NavMesh { get; }
        IUnitAnimatorHandler AnimatorHandler { get; }
    }
}