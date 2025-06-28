using CastleFight.Networking.Models;
using System;
using System.Collections.Generic;

namespace CastleFight.Networking.Handlers
{
    public interface IPlayerListHandler : IEnumerable<NetworkPlayer>
    {
        IObservable<NetworkPlayer> OnPlayerAdded { get; }
        IObservable<NetworkPlayer> OnPlayerRemoved { get; }
        IObservable<NetworkPlayer> OnPlayerUpdated { get; }
        NetworkPlayer LocalPlayer { get; }
        void SetPlayerTeam(ulong clientId, ushort team);
        void SetPlayerGold(ulong clientId, uint gold);
        void SetPlayerReadyStatus(ulong clientId, bool status);
    }
}