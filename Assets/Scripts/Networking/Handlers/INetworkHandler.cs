﻿using System;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace CastleFight.Networking.Handlers
{
    public interface INetworkHandler : ISpawnHandler
    {
        void StartClient(string targetIP, string nickName = null);
        void StartServer(string targetIP, string nickName = null);
        void Disconnect();
        bool IsConnected { get; }
        IObservable<Unit> OnConnected { get; }
        IObservable<Unit> OnDisconnected { get; }
        IPlayerListHandler Players { get; }
        IPingHandler Ping { get; }
        ulong LocalClientId { get; }
        bool IsHost { get; }
        NetworkClient LocalClient { get; }
        IChatHandler Chat { get; }
    }
}