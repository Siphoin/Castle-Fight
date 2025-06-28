using System;
using UniRx;

namespace CastleFight.Core.Handlers
{
    public interface ILobbyHandler
    {
        bool IsStarted { get; }
        IObservable<ushort> OnTick { get; }
        IObservable<Unit> OnEndTick { get; }
        bool AllPlayersIsReady { get; }
    }
}