using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CastleFight.Core.Components;
using CastleFight.Core.Configs;
using CastleFight.Core.Models;
using CastleFight.Networking.Handlers;
using CastleFight.Networking.Models;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace CastleFight.Core.Handlers
{
    public class MatchHandler : NetworkBehaviour, IMatchHandler
    {
        private NetworkVariable<NetworkDateTime> _currentTimeMatch = new();
        private NetworkVariable<NetworkDateTime> _currentTimeSession = new();
        private NetworkVariable<NetworkDictionary<ushort, uint>> _scoresTeams = new();
        private Subject<DateTime> _onTickMatchTime = new();
        private INetworkHandler _networkHandler;
        private CancellationTokenSource _tokenSource;
        private Subject<IReadOnlyDictionary<ushort, uint>> _onTeamsChanged = new();

        private IPointSpawnCastle _leftCastlePoint;
        private IPointSpawnCastle _rightCastlePoint;

        [SerializeField] private MatchConfig _matchConfig;

        public static bool IsSpawnedInstance { get; private set; }

        public DateTime CurrentTimeMatch => _currentTimeMatch.Value.DateTime;
        public DateTime CurrentTimeSession => _currentTimeSession.Value.DateTime;
        public IObservable<DateTime> OnTickMatchTime => _onTickMatchTime;
        public IObservable<IReadOnlyDictionary<ushort, uint>> OnTeamsChanged => _onTeamsChanged;

        public IReadOnlyDictionary<ushort, uint> ScoresTeams => _scoresTeams.Value;

        public override void OnNetworkSpawn()
        {
            IsSpawnedInstance = true;
            _networkHandler = FindAnyObjectByType<NetworkHandler>();
            if (IsHost)
            {
                FindSpawnPointCastles();
                SetupScore();
                StartMatch();
                TickTimeSession().Forget();

            }

            else
            {
                _currentTimeMatch.OnValueChanged += TimeChanged;
                _scoresTeams.OnValueChanged += TeamsScoreChanged;
            }
        }

        private void FindSpawnPointCastles()
        {
            IPointSpawnCastle[] points = FindObjectsByType<PointSpawnCastle>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            _leftCastlePoint = points.FirstOrDefault(x => x.TypeTeam == CastleTeamType.Red);
            _rightCastlePoint = points.FirstOrDefault(x => x.TypeTeam == CastleTeamType.Blue);
        }

        private void SpawnCastle (IPointSpawnCastle point)
        {
            var team = (int)point.TypeTeam;
            var position = point.Position;
            var rotation = point.Rotation;

            _networkHandler.SpawnNetworkObject(_matchConfig.CastlePrefab.gameObject, null, true, (ulong)team, position, rotation);
        }

        private void TeamsScoreChanged(NetworkDictionary<ushort, uint> previousValue, NetworkDictionary<ushort, uint> newValue)
        {
            _onTeamsChanged.OnNext(newValue);
        }

        private void TimeChanged(NetworkDateTime previousValue, NetworkDateTime newValue)
        {
            _onTickMatchTime.OnNext(newValue.DateTime);
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
                SpawnCastles();
                ResetMatchTime();
                ResetGoldPlayers();
            }
        }

        private void SpawnCastles()
        {
            SpawnCastle(_leftCastlePoint);
            SpawnCastle(_rightCastlePoint);
        }

        private void ResetGoldPlayers()
        {
            foreach (var item in _networkHandler.Players)
            {
                var id = item.ClientId;
                _networkHandler.Players.SetPlayerGold(id, _matchConfig.StartGold);
            }
        }

        private void ResetMatchTime()
        {
            DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            _tokenSource?.Cancel();
            _currentTimeMatch.Value = new NetworkDateTime(date);

            TickTimeMatch().Forget();
        }

        private void SetupScore()
        {
            var initialScores = new Dictionary<ushort, uint>
                {
                    { 0, 0 },
                    { 1, 0 }
                };
            _scoresTeams.Value = new NetworkDictionary<ushort, uint>(initialScores);
        }

        private async UniTask TickTimeMatch()
        {
            _tokenSource = new();
            while (true)
            {
                await UniTask.Delay(1000, cancellationToken: _tokenSource.Token);
                var newTime = _currentTimeMatch.Value;
                newTime.AddSeconds(1);
                _currentTimeMatch.Value = newTime;
                _onTickMatchTime.OnNext(CurrentTimeMatch);
            }
        }

        private async UniTask TickTimeSession()
        {
            _tokenSource = new();
            while (true)
            {
                await UniTask.Delay(1000, cancellationToken: _tokenSource.Token);
                var newTime = _currentTimeSession.Value;
                newTime.AddSeconds(1);
                _currentTimeSession.Value = newTime;
            }
        }

        private void OnDisable()
        {
            _tokenSource?.Cancel();
            _currentTimeMatch.OnValueChanged -= TimeChanged;
            _scoresTeams.OnValueChanged -= TeamsScoreChanged;
            IsSpawnedInstance = false;
        }
    }
}