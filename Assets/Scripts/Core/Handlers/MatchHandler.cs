using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CastleFight.Core.Models;
using CastleFight.Networking.Models;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace CastleFight.Core.Handlers
{
    public class MatchHandler : NetworkBehaviour, IMatchHandler
    {
        private NetworkVariable<NetworkDateTime> _currentTime = new();
        private NetworkVariable<NetworkDictionary<ushort, uint>> _scoresTeams = new();
        private Subject<DateTime> _onTickMatchTime = new();
        private CancellationTokenSource _tokenSource;
        private Subject<IReadOnlyDictionary<ushort, uint>> _onTeamsChanged = new();

        public DateTime CurrentTime => _currentTime.Value.DateTime;
        public IObservable<DateTime> OnTickMatchTime => _onTickMatchTime;
        public IObservable<IReadOnlyDictionary<ushort, uint>> OnTeamsChanged => _onTeamsChanged;

        public override void OnNetworkSpawn()
        {
           

            if (IsHost)
            {
                StartMatch();
            }

            else
            {
                _currentTime.OnValueChanged += TimeChanged;
                _scoresTeams.OnValueChanged += TeamsScoreChanged;
            }
        }

        private void TeamsScoreChanged(NetworkDictionary<ushort, uint> previousValue, NetworkDictionary<ushort, uint> newValue)
        {
            _onTeamsChanged.OnNext(newValue);
#if UNITY_EDITOR
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var team in newValue)
            {
                stringBuilder.AppendLine($"{team.Key} {team.Value}");
            }
            Debug.Log(stringBuilder.ToString());
#endif
        }

        private void TimeChanged(NetworkDateTime previousValue, NetworkDateTime newValue)
        {
            _onTickMatchTime.OnNext(newValue.DateTime);
            Debug.Log(newValue.DateTime.ToString("mm:ss"));
        }


        public void ModifyScore(ushort teamId, uint newValue)
        {
            if (IsHost)
            {
                var tempDict = new Dictionary<ushort, uint>(_scoresTeams.Value);
                tempDict[teamId] = newValue;
                _scoresTeams.Value = new(tempDict);
            }
        }

        public void StartMatch()
        {
            if (IsHost)
            {
                var initialScores = new Dictionary<ushort, uint>();
                initialScores.Add(0, 1);
                _scoresTeams.Value = new NetworkDictionary<ushort, uint>(initialScores);

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
            }
        }

        private void OnDisable()
        {
            _tokenSource?.Cancel();
            _currentTime.OnValueChanged -= TimeChanged;
            _scoresTeams.OnValueChanged -= TeamsScoreChanged;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                ModifyScore(0, _scoresTeams.Value.TryGetValue(0, out var current) ? current + 1 : 1);
#if UNITY_EDITOR
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var team in _scoresTeams.Value)
                {
                    stringBuilder.AppendLine($"{team.Key} {team.Value}");
                }
                Debug.Log(stringBuilder.ToString());
#endif
            }
        }
    }
}