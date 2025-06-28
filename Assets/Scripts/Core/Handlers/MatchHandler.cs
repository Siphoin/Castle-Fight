using System;
using System.Threading;
using CastleFight.Core.Models;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace CastleFight.Core.Handlers
{
    public class MatchHandler : NetworkBehaviour, IMatchHandler
    {
        private NetworkVariable<NetworkDateTime> _currentTime = new();
        private Subject<DateTime> _onTickMatchTime = new();
        private CancellationTokenSource _tokenSource;

        public DateTime CurrentTime => _currentTime.Value.DateTime;
        public IObservable<DateTime> OnTickMatchTime => _onTickMatchTime;

        public override void OnNetworkSpawn()
        {
            if (IsHost)
            {
                StartMatch();
            }
            else
            {
                _currentTime.OnValueChanged += TimeChanged;
            }
        }

        private void TimeChanged(NetworkDateTime previousValue, NetworkDateTime newValue)
        {
            _onTickMatchTime.OnNext(newValue.DateTime);
            Debug.Log(newValue.DateTime.ToString("mm:ss"));
        }

        public void StartMatch()
        {
            if (IsHost)
            {
                _tokenSource?.Cancel();
                _currentTime.Value = new NetworkDateTime();
                TickTimeMatch().Forget();
            }
        }

        private async UniTask TickTimeMatch()
        {
            _tokenSource = new();
            while (true)
            {
                await UniTask.Delay(1000, cancellationToken: _tokenSource.Token);

                var newTime = _currentTime.Value;
                newTime.AddSeconds(1);
                _currentTime.Value = newTime;
                Debug.Log(_currentTime.Value.DateTime.ToString("mm:ss"));
            }
        }

        private void OnDisable()
        {
            _tokenSource?.Cancel();
            if (!IsHost)
            {
                _currentTime.OnValueChanged -= TimeChanged;
            }
        }
    }
}