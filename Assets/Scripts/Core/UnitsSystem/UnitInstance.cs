using System.Collections;
using CastleFight.Core.HealthSystem;
using CastleFight.Core.UnitsSystem.Components;
using CastleFight.Networking.Handlers;
using CastleFight.Networking.Models;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace CastleFight.Core.UnitsSystem
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(UnitNavMesh))]
    public class UnitInstance : NetworkBehaviour, IUnitInstance
    {
        [SerializeField, ReadOnly] private HealthComponent _healthComponent;
        [SerializeField, ReadOnly] private UnitNavMesh _navMesh;
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

        IUnitNavMesh IUnitInstance.NavMesh => _navMesh;

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


        }
    }
}