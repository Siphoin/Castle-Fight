using CastleFight.Main.Configs;
using CastleFight.Networking.Handlers;
using CastleFight.Networking.Models;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CastleFight.UI.Views
{
    public class PlayerSlotLobbyView : MonoBehaviour, IPlayerSlotLobbyView
    {
        [SerializeField] private Image _colorImage;
        [SerializeField] private TextMeshProUGUI _textNickName;
        [SerializeField] private Toggle _toggleReady;
        [Inject] private TeamColorPaletteConfig _paletteConfig;
        [Inject] private INetworkHandler _networkHandler;

        private CompositeDisposable _compositeDisposable;

     [field: SerializeReference, ReadOnly]   public ulong PlayerId { get; private set; }

        public void Hide()
        {
            _compositeDisposable?.Clear();
            PlayerId = 0;
            transform.SetParent(null, false);
            gameObject.SetActive(false);
            
        }

        public void SetPlayer(NetworkPlayer player)
        {
            _compositeDisposable?.Clear();
            _compositeDisposable = new();
            _colorImage.color = _paletteConfig.GetColor(player.ColorType);
            _textNickName.text = player.NickName.ToString();
            PlayerId = player.ClientId;
            _toggleReady.interactable = _networkHandler.Players.LocalPlayer.Equals(player);
            _toggleReady.isOn = player.IsReady;
            transform.SetSiblingIndex((int)PlayerId);


            if (_toggleReady.interactable)
            {
                _toggleReady.onValueChanged.AsObservable().Subscribe(value =>
                {

                    _networkHandler.Players.SetPlayerReadyStatus(PlayerId, value);

                }).AddTo(_compositeDisposable);
            }

            else
            {
                _networkHandler.Players.OnPlayerUpdated.Subscribe(player =>
                {
                    if (PlayerId == player.ClientId)
                    {
                        _toggleReady.isOn = player.IsReady;
                    }

                }).AddTo(_compositeDisposable);
            }
        }

        private void OnDisable()
        {
            _compositeDisposable?.Clear();
        }
    }
}