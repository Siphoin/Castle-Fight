using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Subject<NetworkPlayer> _onPlayerUpdated = new();


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

        public NetworkPlayer LocalPlayer => Players.FirstOrDefault(x => x.ClientId == NetworkManager.LocalClient.ClientId);

        public IObservable<NetworkPlayer> OnPlayerUpdated => _onPlayerUpdated;

        private void Awake()
        {
            _players.OnListChanged += HandlePlayersListChanged;;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

            
        }

        

        public override void OnDestroy()
        {
            base.OnDestroy();
            _players.OnListChanged -= HandlePlayersListChanged;

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
            }
        }

        private void HandleClientConnected(ulong clientId)
        {
            if (IsSpawned)
            {
                var isHost = clientId == NetworkManager.ServerClientId;
                var nickName = string.IsNullOrEmpty(NetworkHandler.SetedNickName)
                    ? $"Player_{clientId}"
                    : NetworkHandler.SetedNickName;

                var player = new NetworkPlayer(
                    clientId,
                    new FixedString32Bytes(nickName)
                );

                AddPlayerServerRpc(player);
            }
        }

        private void HandleClientDisconnected(ulong clientId)
        {
            if (IsServer)
            {
                RemovePlayerDirect(clientId);
            }
            else if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                RemovePlayerServerRpc(clientId);
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            HandleClientConnected(NetworkManager.Singleton.LocalClientId);
           
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
                    _onPlayerUpdated.OnNext(changeEvent.Value);
                    Debug.Log($"Player updated: {changeEvent.Value.NickName} (ID: {changeEvent.Value.ClientId})");
             
                    break;

                case NetworkListEvent<NetworkPlayer>.EventType.RemoveAt:
                    _onPlayerRemoved.OnNext(changeEvent.Value);
                    Debug.Log($"Player removed: {changeEvent.Value.NickName} (ID: {changeEvent.Value.ClientId})");
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
            if (IsServer) _players.Add(player);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RemovePlayerServerRpc(ulong clientId)
        {
            RemovePlayerDirect(clientId);
        }

        private void RemovePlayerDirect(ulong clientId)
        {
            for (int i = _players.Count - 1; i >= 0; i--)
            {
                if (_players[i].ClientId == clientId)
                {
                    _onPlayerRemoved.OnNext(_players[i]);
                    _players.RemoveAt(i);
                    break;
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetNickNameServerRpc(ulong clientId, FixedString32Bytes newNickName)
        {
            if (!IsServer) return;

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

        public void SetNickName(ulong clientId, string newNickName)
        {
            SetNickNameServerRpc(clientId, new FixedString32Bytes(newNickName));
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetPlayerTeamServerRpc(ulong clientId, ushort team)
        {
            if (!IsServer) return;

            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId == clientId)
                {
                    NetworkPlayer modifiedPlayer = _players[i];
                    modifiedPlayer.Team = team;
                    _players[i] = modifiedPlayer;
                    break;
                }
            }
        }

        public void SetPlayerTeam(ulong clientId, ushort team)
        {
            SetPlayerTeamServerRpc(clientId, team);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetPlayerGoldServerRpc(ulong clientId, uint gold)
        {

            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId == clientId)
                {
                    NetworkPlayer modifiedPlayer = _players[i];
                    modifiedPlayer.Gold = gold;
                    _players[i] = modifiedPlayer;
                    break;
                }
            }
        }

        public void SetPlayerGold(ulong clientId, uint gold)
        {
            SetPlayerGoldServerRpc(clientId, gold);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetPlayerReadyStatusServerRpc(ulong clientId, bool status)
        {

            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].ClientId == clientId)
                {
                    NetworkPlayer modifiedPlayer = _players[i];
                    modifiedPlayer.IsReady = status;
                    _players[i] = modifiedPlayer;
                    break;
                }
            }
        }

        public void SetPlayerReadyStatus(ulong clientId, bool status)
        {
            SetPlayerReadyStatusServerRpc(clientId, status);
        }

        public NetworkPlayer GetPlayerById (ulong id)
        {
            return Players.FirstOrDefault(x => x.ClientId == id);
        }

        public IEnumerator<NetworkPlayer> GetEnumerator() => Players.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}