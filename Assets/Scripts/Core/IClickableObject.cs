using CastleFight.Core.Graphic;

namespace CastleFight.Core
{
    public interface IClickableObject : ILivingEntity, IPortaable
    {
        string Name { get; }
        string DamageInfo { get; }
        float SelectionScale { get; }
        bool IsSelected { get; }

        void SetStateSelect(bool visible);
    }
}
