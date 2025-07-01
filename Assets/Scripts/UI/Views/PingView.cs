using CastleFight.Networking.Handlers;
using UniRx;
using Zenject;

namespace CastleFight.UI.Views
{
    public class PingView : UIText
    {
        [Inject] private INetworkHandler _network;

        private void Awake()
        {
            _network.Ping.OnPingChanged.Subscribe(ping =>
            {
                Component.text = $"Ping: {ping} ms";

            }).AddTo(this);
        }

        private void Start()
        {
            Component.text = string.Empty;
        }
    }
}