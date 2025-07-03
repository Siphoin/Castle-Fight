using System;
using CastleFight.Networking.Models;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace CastleFight.Networking.Handlers
{
    public class ChatHandler : NetworkBehaviour, IChatHandler
    {
        private INetworkHandler _network;
        private Subject<ChatMessage> _onNewMessage = new();

        public IObservable<ChatMessage> OnNewMessage => _onNewMessage;

        private INetworkHandler Network => _network ??= FindAnyObjectByType<NetworkHandler>();

        public void SendMessageToChat(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            var clientId = Network.Players.LocalPlayer.ClientId;

            if (IsServer)
            {
                BroadcastMessageClientRpc(clientId, message);
            }
            else
            {
                SendMessageServerRpc(clientId, message);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SendMessageServerRpc(ulong senderId, string message, ServerRpcParams rpcParams = default)
        {
            if (senderId != rpcParams.Receive.SenderClientId)
            {
                Debug.LogWarning($"Possible spoofing attempt! Claimed: {senderId}, Actual: {rpcParams.Receive.SenderClientId}");
                return;
            }

            BroadcastMessageClientRpc(senderId, message);
        }

        [ClientRpc]
        private void BroadcastMessageClientRpc(ulong senderId, string message)
        {
            var chatMessage = CreateChatMessage(senderId, message);
            _onNewMessage.OnNext(chatMessage);
        }

        private ChatMessage CreateChatMessage(ulong clientId, string message)
        {
            return new ChatMessage
            {
                Sender = Network.Players.GetPlayerById(clientId),
                Message = message
            };
        }
    }
}