using System;
using System.Collections.Generic;

namespace CastleFight.Core.Handlers
{
    public interface IMatchHandler
    {
        DateTime CurrentTimeMatch { get; }
        IObservable<DateTime> OnTickMatchTime { get; }
        DateTime CurrentTimeSession { get; }
        IReadOnlyDictionary<ushort, uint> ScoresTeams { get; }
        IObservable<IReadOnlyDictionary<ushort, uint>> OnTeamsChanged { get; }


    }
}