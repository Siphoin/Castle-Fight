using CastleFight.Main.Factories;

namespace CastleFight.Core.ConstructionSystem
{
    public interface IBuildingConstructHandler : ILivingEntity
    {
        void TurnConstruct();
    }
}