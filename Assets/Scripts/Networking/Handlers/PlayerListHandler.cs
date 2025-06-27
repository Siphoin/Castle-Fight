using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CastleFight.Networking.Models;
using UniRx;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace CastleFight.Networking.Handlers
{
    public class PlayerListHandler : NetworkBehaviour, IPlayerListHandler
    {
        private readonly NetworkList<NetworkPlayer> _players = new();
        private readonly Subject<NetworkPlayer> _onPlayerAdded = new();
        private readonly Subject<NetworkPlayer> _onPlayerRemoved = new();

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private IEnumerable<NetworkPlayer> Players
        {
            get
            {
                List<NetworkPlayer> list = new List<NetworkPlayer>();

                foreach (NetworkPlayer player in _players)
                {
                    list.Add(player);
                }

                return list;
            }
        }
        public IObservable<NetworkPlayer> OnPlayerAdded => _onPlayerAdded;
        public IObservable<NetworkPlayer> OnPlayerRemoved => _onPlayerRemoved;

        private void Awake()
        {
            _players.OnListChanged += HandlePlayersListChanged;
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            _players.OnListChanged -= HandlePlayersListChanged;
        }

        private void HandlePlayersListChanged(NetworkListEvent<NetworkPlayer> changeEvent)
        {
            switch (changeEvent.Type)
            {
                case NetworkListEvent<NetworkPlayer>.EventType.Add:
                    _onPlayerAdded.OnNext(changeEvent.Value);
                    Debug.Log($"Player added: {changeEvent.Value.NickName} (ID: {changeEvent.Value.ClientId})");
                    break;

                case NetworkListEvent<NetworkPlayer>.EventType.Remove:
                    _onPlayerRemoved.OnNext(changeEvent.Value);
                    Debug.Log($"Player removed: {changeEvent.Value.NickName} (ID: {changeEvent.Value.ClientId})");
                    break;

                case NetworkListEvent<NetworkPlayer>.EventType.Value:
                    // Обработка изменения значений существующего игрока
                    Debug.Log($"Player updated: {changeEvent.Value.NickName} (ID: {changeEvent.Value.ClientId})");
                    break;
            }

#if UNITY_EDITOR
            StringBuilder sb = new StringBuilder();
            sb.Append("Players list:\n");

            foreach (var player in Players)
            {
                sb.AppendLine(player.NickName.ToString());
            }

            Debug.Log(sb.ToString());
#endif
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddPlayerServerRpc(NetworkPlayer player)
        {
            if (IsServer)
            {
                _players.Add(player);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void RemovePlayerServerRpc(ulong clientId)
        {
            if (IsServer)
            {

                foreach (NetworkPlayer player in _players)
                {
                    if (clientId == player.ClientId)
                    {
                        _players.Remove(player);
                    }
                }

                
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetNickNameServerRpc(ulong clientId, FixedString32Bytes newNickName)
        {
            if (IsServer)
            {
                for (int i = 0; i < _players.Count; i++)
                {
                    if (_players[i].ClientId == clientId)
                    {
                        NetworkPlayer modifiedPlayer = _players[i];
                        modifiedPlayer.NickName = newNickName;
                        _players[i] = modifiedPlayer;
                        break;
                    }
                }
            }
        }

        public void SetNickName(ulong clientId, string newNickName)
        {
            SetNickNameServerRpc(clientId, new FixedString32Bytes(newNickName));
        }

        public override void OnNetworkSpawn()
        {
            var defaultNickName = string.IsNullOrEmpty(NetworkHandler.SetedNickName)
                ? $"Player_{NetworkManager.Singleton.LocalClientId}"
                : NetworkHandler.SetedNickName;

            var localPlayer = new NetworkPlayer(
                NetworkManager.Singleton.LocalClientId,
                new FixedString32Bytes(defaultNickName)
            );

            AddPlayerServerRpc(localPlayer);
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient && !IsServer)
            {
                RemovePlayerServerRpc(NetworkManager.Singleton.LocalClientId);
            }
        }

        public IEnumerator<NetworkPlayer> GetEnumerator()
        {
            return Players.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}