using CastleFight.Networking.Models;
using System;
using System.Collections.Generic;

namespace CastleFight.Networking.Handlers
{
    public interface IPlayerListHandler : IEnumerable<NetworkPlayer>
    {
        IObservable<NetworkPlayer> OnPlayerAdded { get; }
        IObservable<NetworkPlayer> OnPlayerRemoved { get; }
    }
}