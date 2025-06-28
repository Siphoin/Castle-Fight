using System;

namespace CastleFight.Core.Handlers
{
    public interface ILobbyHandler
    {
        bool IsStarted { get; }
        IObservable<ushort> OnTick { get; }
    }
}