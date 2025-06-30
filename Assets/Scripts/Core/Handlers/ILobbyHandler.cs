using System;
using UniRx;

namespace CastleFight.Core.Handlers
{
    public interface ILobbyHandler
    {
        bool IsStarted { get; }
        IObservable<ushort> OnTick { get; }
        IObservable<Unit> OnEndTick { get; }
        IObservable<Unit> OnStartTick { get; }
        bool AllPlayersIsReady { get; }
        ushort CurrentTicks { get; }
        bool IsTickingTimer { get; }

        void Turn();
    }
}