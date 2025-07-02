using CastleFight.Core.HealthSystem.Events;
using System;

namespace CastleFight.Core.HealthSystem
{
    public interface IHealthComponent
    {
        bool IsDead { get; }
        IObservable<float> OnCurrentHealthChanged { get; }
        IObservable<DeathEvent> OnDeath { get; }
        IObservable<HitEvent> OnHit { get; }
        IObservable<RegenEvent> OnRegen { get; }
        float Health { get; }
        float MaxHealth { get; }
        void Damage(float damage, ulong damager);
    }
}