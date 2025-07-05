using CastleFight.Core.BuildingsSystem;
using System;
using UniRx;

namespace Core.ConstructionSystem.Handlers
{
    public interface IConstructHandler
    {
        public IObservable<IBuildingInstance> OnSelectBuilding { get; }

        public IObservable<Unit> OnEndBuild { get; }
        void SelectConstruct(IBuildingInstance building);
    }
}