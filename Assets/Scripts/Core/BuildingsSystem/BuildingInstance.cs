using CastleFight.Core.HealthSystem;
using CastleFight.Core.UnitsSystem.Components;
using CastleFight.Networking.Handlers;
using CastleFight.Networking.Models;
using UnityEngine;
using Unity.Netcode;
using Sirenix.OdinInspector;
using UniRx;
using System;
using CastleFight.Core.BuildingsSystem.SO;
using CastleFight.Core.BuildingsSystem.Components;

namespace CastleFight.Core.BuildingsSystem
{
    [RequireComponent(typeof(BuildingSpawnHandler))]
    public class BuildingInstance : NetworkBehaviour, IBuildingInstance
    {
        [SerializeField, ReadOnly] private HealthComponent _healthComponent;
        [SerializeField, ReadOnly] private NetworkHandler _network;
        [SerializeField] private ScriptableBuuidingEntity _stats;
        private NetworkVariable<NetworkPlayer> _owner = new();
        private Subject<NetworkPlayer> _onPlayerOwnerChanged = new();

        private NetworkHandler Network
        {
            get
            {
                if (!_network)
                {
                    _network = FindAnyObjectByType<NetworkHandler>();
                }
                return _network;
            }
        }

        public IHealthComponent HealthComponent => _healthComponent;

        public bool IsMy => Network.Players.LocalPlayer.Equals(_owner.Value);

        public NetworkPlayer Owner => Network.Players.GetPlayerById(_owner.Value.ClientId);

        public ulong OwnerId => OwnerClientId;

        private bool IsOwnerSeted => !string.IsNullOrEmpty(_owner.Value.NickName.ToString());

        public IObservable<NetworkPlayer> OnPlayerOwnerChanged => _onPlayerOwnerChanged;

        public ScriptableBuuidingEntity Stats => _stats;

        private void Start()
        {
            if (IsOwner && !IsOwnerSeted)
            {
                NetworkPlayer networkPlayer = Network.Players.GetPlayerById(OwnerId);
                SetOwner(networkPlayer);
                _healthComponent.SetHealthData(_stats.MaxHealth);
            }

            else if (!IsOwner)
            {
                _owner.OnValueChanged += OwmerChanged;
            }

        }

        private void OwmerChanged(NetworkPlayer previousValue, NetworkPlayer newValue)
        {
            _onPlayerOwnerChanged.OnNext(newValue);
        }

        private void OnValidate()
        {
            if (!_healthComponent)
            {
                _healthComponent = GetComponent<HealthComponent>();
            }
        }

        public void SetOwner(NetworkPlayer owner)
        {
            if (IsServer)
            {
                _owner.Value = owner;
            }

            else
            {
                SetOwnerServerRpc(owner.ClientId);
            }
        }
        [ServerRpc(RequireOwnership = false)]
        public void SetOwnerServerRpc(ulong id)
        {
            NetworkPlayer owner = Network.Players.GetPlayerById(id);
            SetOwner(owner);
        }

        private void OnDisable()
        {
            _owner.OnValueChanged -= OwmerChanged;
        }
    }
}