﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CastleFight.Core.BuildingsSystem;
using CastleFight.Core.BuildingsSystem.Factories;
using CastleFight.Core.Components;
using CastleFight.Core.Configs;
using CastleFight.Core.GameCamer;
using CastleFight.Core.GameCamera;
using CastleFight.Core.Models;
using CastleFight.Core.UnitsSystem;
using CastleFight.Core.UnitsSystem.Factories;
using CastleFight.Networking.Handlers;
using CastleFight.Networking.Models;
using Cysharp.Threading.Tasks;
using ObjectRepositories.Extensions;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using Zenject;

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
        private Subject<ushort> _onWinTeam = new();

        private IPointSpawnCastle _leftCastlePoint;
        private IPointSpawnCastle _rightCastlePoint;

        private IBuildingInstance _redCastle;
        private IBuildingInstance _blueCastle;

        private CompositeDisposable _castleDeadDisposable;

        [SerializeField] private MatchConfig _matchConfig;
        [Inject] private IBuildingFactory _buildingFactory;
        [Inject] private IUnitFactory _unitFactory;
        private IPointCameraView _redTeamCameraPoint;
        private IPointCameraView _blueTeamCameraPoint;

        private RTSCinemachineCamera _gameCamera;

        private RTSCinemachineCamera GameCamera
        {
            get
            {
                if (_gameCamera is null)
                {
                    _gameCamera = FindAnyObjectByType<RTSCinemachineCamera>();
                }
                return _gameCamera;
            }
        }

        private IEnumerable<IPointCameraView> PointsCameraViews
        {
            get
            {
                return FindObjectsByType<PointCameraView>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            }
        }

        public static bool IsSpawnedInstance { get; private set; }

        public DateTime CurrentTimeMatch => _currentTimeMatch.Value.DateTime;
        public DateTime CurrentTimeSession => _currentTimeSession.Value.DateTime;
        public IObservable<DateTime> OnTickMatchTime => _onTickMatchTime;
        public IObservable<IReadOnlyDictionary<ushort, uint>> OnTeamsChanged => _onTeamsChanged;

        public IReadOnlyDictionary<ushort, uint> ScoresTeams => _scoresTeams.Value;

        public IObservable<ushort> OnWinTeam => _onWinTeam;

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
                SpawnWorkers();
            }

            else
            {
                _currentTimeMatch.OnValueChanged += TimeChanged;
                _scoresTeams.OnValueChanged += TeamsScoreChanged;
            }
        }

        private async void Start()
        {
            await FindCameraViews();
            await SetupCameraForTeams();
        }
        private void SpawnWorkers ()
        {
            var points = FindObjectsByType<PointSpawnWorker>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var player in _networkHandler.Players)
            {
                var point = points.FirstOrDefault(x => x.IndexPlayer == (int)player.ClientId);

                if (point != null)
                {
                    Vector3 position = point.Position;
                    Quaternion rotation = point.Rotation;
                   var worker =  _unitFactory.Create(_matchConfig.WorkerPrefab, position, rotation);
                    worker.SetOwner(player);

                }
            }
        }
        private void FindSpawnPointCastles()
        {
            IPointSpawnCastle[] points = FindObjectsByType<PointSpawnCastle>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            _leftCastlePoint = points.FirstOrDefault(x => x.TypeTeam == CastleTeamType.Red);
            _rightCastlePoint = points.FirstOrDefault(x => x.TypeTeam == CastleTeamType.Blue);

        }

        private async UniTask FindCameraViews()
        {
            await UniTask.WaitUntil(() => PointsCameraViews != null && PointsCameraViews.Count() > 0, cancellationToken: this.GetCancellationTokenOnDestroy());
            _redTeamCameraPoint = PointsCameraViews.FirstOrDefault(x => x.TeamType == CastleTeamType.Red);
            _blueTeamCameraPoint = PointsCameraViews.FirstOrDefault(x => x.TeamType == CastleTeamType.Blue);
        }

        private async UniTask SetupCameraForTeams()
        {
            await UniTask.WaitUntil(() => GameCamera != null, cancellationToken: this.GetCancellationTokenOnDestroy());
            var player = _networkHandler.Players.LocalPlayer;
            var point = player.Team == 0 ? _redTeamCameraPoint : _blueTeamCameraPoint;
            _gameCamera.SetFollowTarget(null);
            _gameCamera.transform.SetPositionAndRotation(point.Position, point.Rotation);

        }

        private IBuildingInstance SpawnCastle (IPointSpawnCastle point, ulong ownerId)
        {
            var team = (int)point.TypeTeam;
            var position = point.Position;
            var rotation = point.Rotation;

           var castle = _buildingFactory.Create(_matchConfig.CastlePrefab, position, rotation);
          var owner = _networkHandler.Players.GetPlayerById(ownerId);

            castle.SetOwner(owner);

            return castle;
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
                _onTeamsChanged.OnNext(_scoresTeams.Value);
            }
        }

        public async void StartMatch()
        {
            if (IsHost)
            {
                var token = this.GetCancellationTokenOnDestroy();
                await UniTask.WaitForEndOfFrame(cancellationToken: token);
                SpawnCastles();
                ResetMatchTime();
                ResetGoldPlayers();
            }
        }

        private void SpawnCastles()
        {
            _castleDeadDisposable?.Dispose();

            _castleDeadDisposable = new CompositeDisposable();

            _redCastle = SpawnCastle(_leftCastlePoint, 0);
            _blueCastle = SpawnCastle(_rightCastlePoint, 1);


            _redCastle.HealthComponent.OnCurrentHealthChanged.Subscribe(health =>
            {
                if (health <= 0)
                {
                    WinTeam(1);
                    _castleDeadDisposable?.Dispose();
                }
            }).AddTo(_castleDeadDisposable);

            _blueCastle.HealthComponent.OnCurrentHealthChanged.Subscribe(health =>
            {
                if (health <= 0)
                {
                    WinTeam(0);
                    _castleDeadDisposable?.Dispose();
                }
            }).AddTo(_castleDeadDisposable);
        }

        private void WinTeam(ushort teamId)
        {
            RemoveAllBuildings();
            RemoveAllUnits();
            uint currentScore = _scoresTeams.Value[teamId] + 1;
            ModifyScore(teamId, currentScore);
            _onWinTeam.OnNext(teamId);
            WinTeamClientRpc(teamId);
            StartMatch();
        }

        private void RemoveAllUnits()
        {
            var units = this.FindManyByConditionOnRepository<UnitInstance>(x => x.Isinvulnerable == false).ToArray();
            for (uint i = 0; i < units.Length; i++)
            {
                _networkHandler.DestroyNetworkObject(units[i].gameObject);
            }
        }

        private void RemoveAllBuildings()
        {
            var buildings = this.FindObjectsOfTypeOnRepository<BuildingInstance>().ToArray();
            for (int i = 0; i < buildings.Length; i++)
            {
                _networkHandler.DestroyNetworkObject(buildings[i].gameObject);
            }
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

        [ClientRpc]
        private void WinTeamClientRpc(ushort teamId)
        {
            _onWinTeam.OnNext(teamId);
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