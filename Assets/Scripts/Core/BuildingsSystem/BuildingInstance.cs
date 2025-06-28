using CastleFight.Core.HealthSystem;
using CastleFight.Core.UnitsSystem.Components;
using CastleFight.Networking.Handlers;
using CastleFight.Networking.Models;
using UnityEngine;
using Unity.Netcode;
using Sirenix.OdinInspector;

namespace CastleFight.Core.BuildingsSystem
{
    public class BuildingInstance : NetworkBehaviour, IBuildingInstance
    {
        [SerializeField, ReadOnly] private HealthComponent _healthComponent;
        [SerializeField, ReadOnly] private NetworkHandler _network;
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

        public bool IsMy => IsOwner;

        public NetworkPlayer Owner => Network.Players.GetPlayerById(OwnerId);

        public ulong OwnerId => OwnerClientId;

        private void OnValidate()
        {
            if (!_healthComponent)
            {
                _healthComponent = GetComponent<HealthComponent>();
            }
        }
    }
}