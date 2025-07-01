using CastleFight.Core.BuildingsSystem;
using CastleFight.Core.Configs;
using CastleFight.Networking.Handlers;
using CastleFight.Networking.Models;
using Cysharp.Threading.Tasks;
using ObjectRepositories.Extensions;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace CastleFight.Core.Handlers
{
    public class IncomeHandler : NetworkBehaviour
    {
        [SerializeField] private IncomeHandlerConfig _config;

        [SerializeField, ReadOnly] private NetworkHandler _network;

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


        public override void OnNetworkSpawn()
        {
           if (IsServer)
            {
                TickIncome().Forget();
            }
        }

        private async UniTask TickIncome ()
        {
            var token = this.GetCancellationTokenOnDestroy();

            while (true)
            {
                await UniTask.Delay(_config.TimeIncome, cancellationToken: token);
                foreach (var player in Network.Players)
                {
                    IncomeToPlayer(player);
                }
            }
        }

        private void IncomeToPlayer (NetworkPlayer player)
        {
            var buildings = this.FindManyByConditionOnRepository<BuildingInstance>(x => x.Owner.Equals(player));
            uint totalIncome = 0;

            foreach (var building in buildings)
            {
                totalIncome += building.Stats.Income;
            }

            var result = player.Gold += totalIncome;
            Network.Players.SetPlayerGold(player.ClientId, result);

        }
    }
}