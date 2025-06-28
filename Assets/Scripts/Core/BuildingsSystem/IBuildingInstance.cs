using CastleFight.Main.Factories;

namespace CastleFight.Core.BuildingsSystem
{
    public interface IBuildingInstance : IFactoryObject, ILivingEntity, IOwnerable
    {
    }
}