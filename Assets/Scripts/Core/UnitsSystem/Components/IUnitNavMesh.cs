using CastleFight.Core.Components;
using CastleFight.Core.HealthSystem;

namespace CastleFight.Core.UnitsSystem.Components
{
    public interface IUnitNavMesh : IDisableComponent
    {
        void SetTarget (HealthComponent healthComponent);
        void Move();
        void Stop();

        IHealthComponent CurrentTarget { get; }
    }
}