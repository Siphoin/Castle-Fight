using CastleFight.Networking.Handlers;
using Zenject;
using UniRx;
using CastleFight.Extensions;
using System;
namespace CastleFight.UI.Views
{
    public class GoldView : UIText
    {
        [Inject] private INetworkHandler _network;
        private Guid _idPlayer;

        private void Awake()
        {
            _idPlayer = _network.Players.LocalPlayer.Guid;
            _network.Players.OnPlayerUpdated.Subscribe(player =>
            {
                if (player.Guid == _idPlayer)
                {
                    UpdateValue();
                }

            }).AddTo(this);
        }

        private void Start()
        {
            UpdateValue();
        }



        private void UpdateValue ()
        {
            if (!_network.IsConnected)
            {
                return;
            }
            Component.text = _network.Players.LocalPlayer.Gold.ToGameCurrency();
        }
    }
}