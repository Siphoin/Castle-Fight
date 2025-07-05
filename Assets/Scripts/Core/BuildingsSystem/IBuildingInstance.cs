using System;
using CastleFight.Core.BuildingsSystem.SO;
using CastleFight.Core.ConstructionSystem.Views;
using CastleFight.Core.SO;
using CastleFight.Main.Factories;

namespace CastleFight.Core.BuildingsSystem
{
    public interface IBuildingInstance : IFactoryObject, ILivingEntity, IOwnerable, IStatsableEntity<ScriptableBuuidingEntity>, IClickableObject
    {
        void TurnConstruct();
        IObservable<bool> OnStartConstruct { get; }
        IBuildingModelView BuildingView { get; }
        bool IsContructed { get; }
    }
}