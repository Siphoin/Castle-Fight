using System;

namespace CastleFight.Networking.Handlers
{
    public interface IPingHandler
    {
        IObservable<int> OnPingChanged { get; }
    }
}