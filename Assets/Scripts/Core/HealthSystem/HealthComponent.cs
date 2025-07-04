using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using UniRx;
using CastleFight.Core.HealthSystem.Events;
using System;
using System.Linq;
using Zenject;
using CastleFight.Core.HealthSystem.Configs;
using CastleFight.Core.BuildingsSystem;

namespace CastleFight.Core.HealthSystem
{
    [RequireComponent(typeof(HealthComponentObjectRegistrer))]
    public class HealthComponent : NetworkBehaviour, IHealthComponent
    {
        [SerializeField, ReadOnly] private NetworkVariable<float> _currentHealth = new(100);
        [SerializeField, ReadOnly] private NetworkVariable<float> _maxHealth = new(100);
        [SerializeField] private NetworkVariable<DeathEvent> _deathEvent = new(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
        [Inject] private HealthComponentConfig _config;

        private Subject<float> _onCurrentHealthChanged = new();
        private Subject<DeathEvent> _onDeath = new();
        private Subject<HitEvent> _onHit = new();
        private Subject<RegenEvent> _onRegen = new();

#if UNITY_EDITOR
        [Button(60)]
        private void KillDebug ()
        {
            if (IsOwner)
            {
                Kill();
            }
        }
#endif

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
            _deathEvent.OnValueChanged += OnDeathChanged;
        }

        private void OnDeathChanged(DeathEvent previousValue, DeathEvent newValue)
        {
            _currentHealth.OnValueChanged -= HealthChanged;
            _deathEvent.OnValueChanged -= OnDeathChanged;

            _onDeath.OnNext(_deathEvent.Value);

            var networkObject = NetworkManager.SpawnManager.SpawnedObjects.FirstOrDefault(x => x.Key == _deathEvent.Value.IdObject).Value;

            if (networkObject != null)
            {
                Debug.Log($"unit {networkObject.name} kill {name} Killer ID {networkObject.NetworkObjectId}");
            }
        }

        

        private void HealthChanged(float previousValue, float newValue)
        {
            _onCurrentHealthChanged.OnNext(newValue);
        }

        public void Damage(float damage, ulong damager)
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
                DamageServerRpc(damage, damager);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void DamageServerRpc(float damage, ulong objectId)
        {
            object damager = null;

            if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var networkObject))
            {
                damager = networkObject.GetComponent<NetworkBehaviour>();
            }
            ApplyDamage(damage, damager);
        }

        private void ApplyDamage(float damage, object damager)
        {
            _currentHealth.Value = Mathf.Clamp(_currentHealth.Value - damage, 0, _maxHealth.Value);
            HitEvent hitEvent = new HitEvent(damage, damager);
            _onHit.OnNext(hitEvent);

            if (_currentHealth.Value <= 0 && IsOwner)
            {
                _deathEvent.Value = new();

                if (damager is NetworkBehaviour behaviour)
                {
                    ulong idObject = behaviour.NetworkObjectId;
                    _deathEvent.Value = new(idObject);
                }
                _onDeath.OnNext(_deathEvent.Value);
            }
        }

        public void SetHealth(float health)
        {
            if (IsDead) return;

            if (IsServer)
            {
                ApplySetHealth(health);
            }
            else
            {
                SetHealthServerRpc(health);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetHealthServerRpc(float health, ServerRpcParams rpcParams = default)
        {
            ApplySetHealth(health);
        }

        private void ApplySetHealth(float health)
        {
            float newHealth = Mathf.Clamp(health, 0, _maxHealth.Value);
            float difference = newHealth - _currentHealth.Value;

            _currentHealth.Value = newHealth;
            _onCurrentHealthChanged.OnNext(_currentHealth.Value);

            if (difference > 0)
            {
                RegenEvent regenEvent = new RegenEvent(difference);
                _onRegen.OnNext(regenEvent);
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

        public void Kill ()
        {
            Damage(float.MaxValue, 0);
        }

        public void TurnConstructHealth ()
        {
            if (TryGetComponent(out IBuildingInstance _))
            {
                _currentHealth.Value = _config.StartHealthConstructBuilding;

            }

            else
            {
                Debug.LogError($"health component owner not a Building");
            }
        }

    }
}