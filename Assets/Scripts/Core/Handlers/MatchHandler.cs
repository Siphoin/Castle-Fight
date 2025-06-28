using System;
using CastleFight.Core.Models;
using UniRx;
using Unity.Netcode;

namespace CastleFight.Core.Handlers
{
    public class MatchHandler : NetworkBehaviour, IMatchHandler
    {
        private NetworkVariable<NetworkDateTime> _currentTime = new();
        private Subject<DateTime> _onTickMatchTime = new();

        public DateTime CurrentTime => _currentTime.Value.DateTime;

        public IObservable<DateTime> OnTickMatchTime => _onTickMatchTime;
    }
}