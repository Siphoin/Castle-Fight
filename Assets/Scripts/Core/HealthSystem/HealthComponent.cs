using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using UniRx;
using CastleFight.Core.HealthSystem.Events;
using System;

namespace CastleFight.Core.HealthSystem
{
    [RequireComponent(typeof(HealthComponentObjectRegistrer))]
    public class HealthComponent : NetworkBehaviour, IHealthComponent
    {
        [SerializeField, ReadOnly] private NetworkVariable<float> _currentHealth = new(100);
        [SerializeField, ReadOnly] private NetworkVariable<float> _maxHealth = new(100);

        private Subject<float> _onCurrentHealthChanged = new();
        private Subject<DeathEvent> _onDeath = new();
        private Subject<HitEvent> _onHit = new();
        private Subject<RegenEvent> _onRegen = new();

        public float Health => _currentHealth.Value;
        public float MaxHealth => _maxHealth.Value;

        public bool IsDead => _currentHealth.Value <= 0;

        public IObservable<float> OnCurrentHealthChanged => _onCurrentHealthChanged;
        public IObservable<DeathEvent> OnDeath => _onDeath;
        public IObservable<HitEvent> OnHit => _onHit;

        public IObservable<RegenEvent> OnRegen => _onRegen;

        public override void OnNetworkSpawn()
        {
            _currentHealth.OnValueChanged += HealthChanged;
        }

        private void HealthChanged(float previousValue, float newValue)
        {
            _onCurrentHealthChanged.OnNext(newValue);
            if (newValue <= 0)
            {
                DeathEvent deathEvent = new DeathEvent();
                _onDeath.OnNext(deathEvent);
            }
        }

        public void Damage(float damage, object damager = null)
        {
            if (IsDead)
            {
                return;
            }

            if (IsServer)
            {
                ApplyDamage(damage, damager);
            }
            else
            {
                DamageServerRpc(damage);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void DamageServerRpc(float damage)
        {
            ApplyDamage(damage, null);
        }

        private void ApplyDamage(float damage, object damager)
        {
            _currentHealth.Value = Mathf.Clamp(_currentHealth.Value - damage, 0, _maxHealth.Value);
            HitEvent hitEvent = new HitEvent(damage, damager);
            _onHit.OnNext(hitEvent);
            _onCurrentHealthChanged.OnNext(_currentHealth.Value);

            if (_currentHealth.Value <= 0)
            {
                DeathEvent deathEvent = new DeathEvent(damager);
                _onDeath.OnNext(deathEvent);
            }
        }

        public void Regen(float amount)
        {
            if (IsDead)
            {
                return;
            }

            if (IsServer)
            {
                ApplyRegen(amount);
            }
            else
            {
                RegenServerRpc(amount);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void RegenServerRpc(float amount, ServerRpcParams rpcParams = default)
        {
            ApplyRegen(amount);
        }

        private void ApplyRegen(float amount)
        {
            _currentHealth.Value = Mathf.Clamp(_currentHealth.Value + amount, 0, _maxHealth.Value);
            _onCurrentHealthChanged.OnNext(_currentHealth.Value);
            RegenEvent regenEvent = new RegenEvent(amount);
            _onRegen.OnNext(regenEvent);
        }

        public void SetHealthData(float maxHealth)
        {
            if (IsServer)
            {
                ApplyHealthData(maxHealth);
            }
            else
            {
                SetHealthDataServerRpc(maxHealth);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetHealthDataServerRpc(float maxHealth, ServerRpcParams rpcParams = default)
        {
            ApplyHealthData(maxHealth);
        }

        private void ApplyHealthData(float maxHealth)
        {
            _maxHealth.Value = maxHealth;
            _currentHealth.Value = maxHealth;

            _onCurrentHealthChanged.OnNext(_currentHealth.Value);
        }

    }
}