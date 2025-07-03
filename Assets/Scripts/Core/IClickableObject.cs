using CastleFight.Core.HealthSystem;

namespace CastleFight.Core
{
    public interface IClickableObject : ILivingEntity
    {
        string Name { get; }
        string DamageInfo { get; }
    }
}
