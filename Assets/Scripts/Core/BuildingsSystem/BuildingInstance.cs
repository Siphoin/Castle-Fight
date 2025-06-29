using CastleFight.Core.HealthSystem;
using CastleFight.Core.UnitsSystem.Components;
using CastleFight.Networking.Handlers;
using CastleFight.Networking.Models;
using UnityEngine;
using Unity.Netcode;
using Sirenix.OdinInspector;
using UniRx;
using System;

namespace CastleFight.Core.BuildingsSystem
{
    public class BuildingInstance : NetworkBehaviour, IBuildingInstance
    {
        [SerializeField, ReadOnly] private HealthComponent _healthComponent;
        [SerializeField, ReadOnly] private NetworkHandler _network;
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

        public bool IsMy => _owner.Value.Equals(Network.Players.LocalPlayer);

        public NetworkPlayer Owner => Network.Players.GetPlayerById(_owner.Value.ClientId);

        public ulong OwnerId => OwnerClientId;

        private bool IsOwnerSeted => !string.IsNullOrEmpty(_owner.Value.NickName.ToString());

        public IObservable<NetworkPlayer> OnPlayerOwnerChanged => _onPlayerOwnerChanged;

        private void Start()
        {
            if (IsOwner && !IsOwnerSeted)
            {
                NetworkPlayer networkPlayer = Network.Players.GetPlayerById(OwnerId);
                SetOwner(networkPlayer);
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