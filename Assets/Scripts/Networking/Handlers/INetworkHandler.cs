using System;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace CastleFight.Networking.Handlers
{
    public interface INetworkHandler : ISpawnHandler
    {
        void StartClient(string targetIP);
        void StartServer();
        void Disconnect();
        bool IsConnected { get; }
        IObservable<Unit> OnConnected { get; }
        IObservable<Unit> OnDisconnected { get; }
        IPlayerListHandler Players { get; }
        ulong LocalClientId { get; }
        NetworkClient LocalClient { get; }
    }
}