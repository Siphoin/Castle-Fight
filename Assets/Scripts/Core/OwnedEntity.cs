using CastleFight.Networking.Handlers;
using CastleFight.Networking.Models;
using Unity.Netcode;
using UniRx;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace CastleFight.Core
{
    public abstract class OwnedEntity : NetworkBehaviour, IOwnerable, ITeamableObject
    {
        [SerializeField, ReadOnly] protected NetworkHandler _network;
        [SerializeField] protected NetworkVariable<NetworkPlayer> _owner = new();
        protected Subject<NetworkPlayer> _onPlayerOwnerChanged = new();

        protected NetworkHandler Network
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

        public bool IsMy => _owner.Value.Equals(Network.Players.LocalPlayer);
        public NetworkPlayer Owner => Network.Players.GetPlayerById(OwnerId);
        public ulong OwnerId => OwnerClientId;
        protected bool IsOwnerSeted => !string.IsNullOrEmpty(_owner.Value.NickName.ToString());
        public IObservable<NetworkPlayer> OnPlayerOwnerChanged => _onPlayerOwnerChanged;

        protected virtual void Start()
        {
            if (IsOwner && !IsOwnerSeted)
            {
                NetworkPlayer networkPlayer = Network.Players.GetPlayerById(OwnerId);
                SetOwner(networkPlayer);
            }
            else if (!IsOwner)
            {
                _owner.OnValueChanged += OwnerChanged;
            }
        }

        protected virtual void OwnerChanged(NetworkPlayer previousValue, NetworkPlayer newValue)
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
        }

        protected virtual void OnDisable()
        {
            _owner.OnValueChanged -= OwnerChanged;
        }

        public bool IsAlly(ITeamableObject other)
        {
            return other.Owner.Team == _owner.Value.Team;
        }

        public bool IsEnemy(ITeamableObject other)
        {
            return other.Owner.Team != _owner.Value.Team;
        }

        public bool IsAlly(IOwnerable other)
        {
            return other.Owner.Team == _owner.Value.Team;
        }

        public bool IsEnemy(IOwnerable other)
        {
            return other.Owner.Team != _owner.Value.Team;
        }
    }
}