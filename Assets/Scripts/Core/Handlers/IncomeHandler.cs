using System.Threading;
using CastleFight.Core.BuildingsSystem;
using CastleFight.Core.Configs;
using CastleFight.Networking.Handlers;
using CastleFight.Networking.Models;
using Cysharp.Threading.Tasks;
using ObjectRepositories.Extensions;
using Sirenix.OdinInspector;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace CastleFight.Core.Handlers
{
    public class IncomeHandler : NetworkBehaviour
    {
        [SerializeField] private IncomeHandlerConfig _config;

        [SerializeField, ReadOnly] private NetworkHandler _network;
        [SerializeField, ReadOnly] private MatchHandler _matchHandler;
        private CancellationTokenSource _incomeCts;

        private INetworkHandler Network
        {
            get
            {

                if (_network is null)
                {
                    _network = FindAnyObjectByType<NetworkHandler>();
                }
                return _network;
            }
        }

        private IMatchHandler MatchHandler
        {
            get
            {
                if (_matchHandler is null)
                {
                    _matchHandler = FindAnyObjectByType<MatchHandler>();
                }
                return _matchHandler;
            }
        }


        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                MatchHandler.OnWinTeam.Subscribe(_ =>
                {
                    Restart();

                }).AddTo(this);
                StartIncomeTicking();
            }
        }

        private void StartIncomeTicking()
        {
            _incomeCts?.Cancel();

            TickIncome().Forget();
        }

        private async UniTask TickIncome ()
        {
            _incomeCts = new();

            while (true)
            {
                await UniTask.Delay(_config.TimeIncome, cancellationToken: _incomeCts.Token);
                foreach (var player in Network.Players)
                {
                    IncomeToPlayer(player);
                }
            }
        }

        private void IncomeToPlayer (NetworkPlayer player)
        {
            var buildings = this.FindManyByConditionOnRepository<BuildingInstance>(x => x.Owner.Equals(player) && x.IsContructed);
            uint totalIncome = 0;

            foreach (var building in buildings)
            {
                totalIncome += building.Stats.Income;
            }

            var result = player.Gold += totalIncome;
            Network.Players.SetPlayerGold(player.ClientId, result);

        }

        private void Restart ()
        {
            _incomeCts?.Cancel();
            TickIncome().Forget();
        }

        public override void OnNetworkDespawn()
        {
            _incomeCts?.Cancel();
        }
    }
}