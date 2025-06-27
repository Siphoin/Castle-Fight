using CastleFight.Networking.Handlers;
using CastleFight.UI.Handlers;
using UnityEngine;
using Zenject;
using UniRx;
namespace CastleFight.UI.Observers
{
    public class ConnectObserver : MonoBehaviour
    {
        [Inject] private INetworkHandler _network;
        [Inject] private IScreenHandler _screenHandler;

        private void OnEnable()
        {
            _network.OnConnected.Subscribe(_ =>
            {
                _screenHandler.SetScreen(ScreenType.LobbyScreen);
            }).AddTo(this);
        }
    }
}