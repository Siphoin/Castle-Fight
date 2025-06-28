using CastleFight.Core.HealthSystem;

namespace CastleFight.Core.UnitsSystem.Components
{
    public interface IUnitNavMesh
    {
        void SetTarget (HealthComponent healthComponent);
        IHealthComponent CurrentTarget { get; }
    }
}