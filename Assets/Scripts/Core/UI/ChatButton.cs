using System.Collections;
using CastleFight.Core.Views.Chat;
using CastleFight.Networking.Handlers;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CastleFight.Core.UI
{
    [RequireComponent(typeof(Button))]
    public class ChatButton : MonoBehaviour
    {
        [Inject] private INetworkHandler _networkHandler;
        [SerializeField, ReadOnly] private Button _button;
        [SerializeField] private Image _notify;
        [SerializeField] private ChatView _chatView;
        [SerializeField] private bool _useNotify = true;
        private IChatHandler Chat => _networkHandler.Chat;

        private void Awake()
        {
            _notify.gameObject.SetActive(false);
            if (_useNotify)
            {
                Chat.OnNewMessage.Subscribe(player =>
                {
                    if (!player.Sender.Equals(_networkHandler.Players.LocalPlayer))
                    {

                        _notify.gameObject.SetActive(true);
                    }

                }).AddTo(this);
            }

            _button.onClick.AsObservable().Subscribe(_ =>
            {
                _chatView.SetStateActive(!_chatView.IsActive);

                if (_chatView.IsActive)
                {
                    _notify.gameObject.SetActive(false);
                }

                gameObject.SetActive(false);

            }).AddTo(this);
        }

        private void OnValidate()
        {
            if (_button is null)
            {
                _button = GetComponent<Button>();
            }
        }

    }
}