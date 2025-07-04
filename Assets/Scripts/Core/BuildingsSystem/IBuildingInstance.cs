using CastleFight.Core.BuildingsSystem.SO;
using CastleFight.Core.SO;
using CastleFight.Main.Factories;

namespace CastleFight.Core.BuildingsSystem
{
    public interface IBuildingInstance : IFactoryObject, ILivingEntity, IOwnerable, IStatsableEntity<ScriptableBuuidingEntity>, IClickableObject
    {
        void TurnConstruct();
    }
}