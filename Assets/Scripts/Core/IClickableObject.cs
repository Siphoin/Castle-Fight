using CastleFight.Core.Graphic;

namespace CastleFight.Core
{
    public interface IClickableObject : ILivingEntity, IPortaable
    {
        string Name { get; }
        string DamageInfo { get; }

        void SetStateSelect(bool visible);
    }
}
