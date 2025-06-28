using System.Collections;
using CastleFight.Core.Handlers;
using CastleFight.Networking.Handlers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using System.Linq;
namespace CastleFight.Core.Views
{
    public class LobbyView : MonoBehaviour
    {
        [SerializeField] private Button _burronStart;
        private ILobbyHandler _lobbyHandler;
        [Inject] private INetworkHandler _networkHandler;

        private void Start()
        {
            _lobbyHandler =  FindAnyObjectByType<LobbyHandler>();
            if (_networkHandler.IsHost)
            {
                _burronStart.interactable = false;
                _networkHandler.Players.OnPlayerUpdated.Subscribe(_ =>
                {
                    UpdateButtonActive();

                }).AddTo(this);

                _networkHandler.Players.OnPlayerRemoved.Subscribe(_ =>
                {
                    UpdateButtonActive();

                }).AddTo(this);

                _networkHandler.Players.OnPlayerAdded.Subscribe(_ =>
                {
                    UpdateButtonActive();

                }).AddTo(this);

                _burronStart.onClick.AsObservable().Subscribe(_ =>
                {
                    _burronStart.interactable = false;
                    _lobbyHandler.Turn();
                }).AddTo(this);
            }

            else
            {
                _burronStart.interactable = false;
            }
        }

        private void UpdateButtonActive()
        {
            _burronStart.interactable = _lobbyHandler.AllPlayersIsReady && _networkHandler.Players.Count() > 0;
        }
    }
}