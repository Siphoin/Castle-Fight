using CastleFight.Core.Handlers;
using CastleFight.Networking.Handlers;
using Unity.Netcode;
using UniRx;
namespace CastleFight.Core.Observers
{
    public class TeleportToGameSceneObserver : NetworkBehaviour
    {
        private ILobbyHandler _lobbyHandler;
        private INetworkHandler _networkHandler;

        private CompositeDisposable _disposable;

        private void Start()
        {
           // DontDestroyOnLoad(gameObject);
        }
        public override void OnNetworkSpawn()
        {
            _lobbyHandler = FindAnyObjectByType<LobbyHandler>();
            _networkHandler = FindAnyObjectByType<NetworkHandler>();
            if (_networkHandler.IsHost)
            {
                _disposable = new();
                _lobbyHandler.OnEndTick.Subscribe(_ =>
                {
                    NetworkManager.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
                    _disposable.Clear();

                }).AddTo(_disposable);
            }
        }
    }
}