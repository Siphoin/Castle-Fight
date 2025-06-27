using System;
using UniRx;

namespace CastleFight.Networking.Handlers
{
    public interface INetworkHandler
    {
        void StartClient(string targetIP);
        void StartServer();
        void Disconnect();
        bool IsConnected { get; }
        IObservable<Unit> OnConnected { get; }
        IObservable<Unit> OnDisconnected { get; }
    }
}