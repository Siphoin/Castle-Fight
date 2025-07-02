using CastleFight.Core.HealthSystem;
using System;

namespace CastleFight.Core.UnitsSystem.Components
{
    public interface IUnitCombatSystem
    {
        IObservable<IHealthComponent> OnKill { get; }
        void Damage();
    }
}