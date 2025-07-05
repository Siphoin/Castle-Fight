using CastleFight.Core.BuildingsSystem;
using CastleFight.Core.ConstructionSystem.Events;
using System;
using UniRx;
using UnityEngine;

namespace Core.ConstructionSystem.Handlers
{
    public interface IConstructHandler
    {
        public IObservable<IBuildingInstance> OnSelectBuilding { get; }

        public IObservable<NewBuildingConstructEvent> OnEndBuild { get; }
        void SelectConstruct(IBuildingInstance building);
    }
}