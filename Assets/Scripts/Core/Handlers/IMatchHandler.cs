using System;

namespace CastleFight.Core.Handlers
{
    public interface IMatchHandler
    {
        DateTime CurrentTime { get; }
        IObservable<DateTime> OnTickMatchTime { get; }
    }
}