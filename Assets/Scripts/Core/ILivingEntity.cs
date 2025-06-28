using CastleFight.Core.HealthSystem;

namespace CastleFight.Core
{
    public interface ILivingEntity
    {
        IHealthComponent HealthComponent { get; }
    }
}
