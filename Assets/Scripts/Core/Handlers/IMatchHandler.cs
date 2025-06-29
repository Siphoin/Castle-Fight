using System;

namespace CastleFight.Core.Handlers
{
    public interface IMatchHandler
    {
        DateTime CurrentTimeMatch { get; }
        IObservable<DateTime> OnTickMatchTime { get; }
        DateTime CurrentTimeSession { get; }
        
    }
}