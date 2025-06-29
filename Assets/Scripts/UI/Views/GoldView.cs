using CastleFight.Networking.Handlers;
using Zenject;
using UniRx;
using CastleFight.Extensions;
namespace CastleFight.UI.Views
{
    public class GoldView : UIText
    {
        [Inject] private INetworkHandler _network;
        private ulong _idPlayer;

        private void Awake()
        {
            _idPlayer = _network.LocalClientId;
            _network.Players.OnPlayerUpdated.Subscribe(player =>
            {
                if (player.ClientId != _idPlayer)
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