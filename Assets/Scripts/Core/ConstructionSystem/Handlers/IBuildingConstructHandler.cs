using System;
using CastleFight.Main.Factories;
using UniRx;

namespace CastleFight.Core.ConstructionSystem
{
    public interface IBuildingConstructHandler : ILivingEntity
    {
        void TurnConstruct();
        bool ConstructProcess { get; set; }
    }
}