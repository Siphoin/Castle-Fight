using System;
using CastleFight.Core.HealthSystem;
using CastleFight.Core.UnitsSystem.Components;
using CastleFight.Core.UnitsSystem.SO;
using CastleFight.Networking.Handlers;
using CastleFight.Networking.Models;
using Sirenix.OdinInspector;
using UniRx;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace CastleFight.Core.UnitsSystem
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(UnitNavMesh))]
    [RequireComponent(typeof(NetworkTransform))]
    public class UnitInstance : NetworkBehaviour, IUnitInstance
    {
        [SerializeField, ReadOnly] private HealthComponent _healthComponent;
        [SerializeField, ReadOnly] private UnitNavMesh _navMesh;
        [SerializeField, ReadOnly] private NetworkHandler _network;
        [SerializeField, ReadOnly] private UnitAnimatorHandler _unitAnimatorHandler;
        [SerializeField] private ScriptableUnitEntity _stats;
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

        public NetworkPlayer Owner => Network.Players.GetPlayerById(OwnerId); 

        public ulong OwnerId => OwnerClientId;

        public IUnitNavMesh NavMesh => _navMesh;

        public IUnitAnimatorHandler AnimatorHandler => _unitAnimatorHandler;

        private bool IsOwnerSeted => !string.IsNullOrEmpty(_owner.Value.NickName.ToString());

        public IObservable<NetworkPlayer> OnPlayerOwnerChanged => _onPlayerOwnerChanged;

        public ScriptableUnitEntity Stats => _stats;

        protected override void OnNetworkPostSpawn()
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

        public void SetOwner(NetworkPlayer owner)
        {
            if (IsServer)
            {
                _owner.Value = owner;
                _onPlayerOwnerChanged.OnNext(owner);
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
            _onPlayerOwnerChanged.OnNext(owner);
        }

        private void OnValidate()
        {
            if (!_healthComponent)
            {
                _healthComponent = GetComponent<HealthComponent>();
            }

            if (!_navMesh)
            {
                _navMesh = GetComponent<UnitNavMesh>();
            }

            if (!_unitAnimatorHandler)
            {
                _unitAnimatorHandler = GetComponentInChildren<UnitAnimatorHandler>();
            }


        }

        private void OnDisable()
        {
            _owner.OnValueChanged -= OwmerChanged;
        }

    }
}