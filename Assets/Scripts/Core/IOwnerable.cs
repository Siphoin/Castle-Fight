using System;
using CastleFight.Networking.Models;

namespace CastleFight.Core
{
    public interface IOwnerable
    {
        bool IsMy {  get; }
        NetworkPlayer Owner { get; }
        ulong OwnerId { get; }
        IObservable<NetworkPlayer> OnPlayerOwnerChanged { get; }

        public void SetOwner(NetworkPlayer owner);
    }
}
