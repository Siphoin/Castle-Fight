using System.Collections;
using System.Text;
using System.Threading.Tasks;
using CastleFight.Main.Configs;
using CastleFight.Networking.Handlers;
using CastleFight.Networking.Models;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CastleFight.Core.Views.Chat
{
    public class ChatView : MonoBehaviour, IChatView
    {
        private StringBuilder _containerMessages = new();
        [SerializeField] private TextMeshProUGUI _textChat;
        [SerializeField] private Button _buttonClear;
        [SerializeField] private Button _buttonSend;
        [SerializeField] private TMP_InputField _input;
        [Inject] private TeamColorPaletteConfig _teamPalette;
        [Inject] private INetworkHandler _networkHandler;

        private IChatHandler Chat => _networkHandler.Chat;

        public bool IsActive => gameObject.activeSelf;

        private async void Awake()
        {
            _textChat.text = string.Empty;
            await UniTask.WaitUntil(() => Chat != null, cancellationToken: this.GetCancellationTokenOnDestroy());
            Chat.OnNewMessage.Subscribe(message =>
            {
                AddMessageToChat(message.Sender, message.Message);
                UpdateChatView();
            }).AddTo(this);

            _buttonClear.onClick.AsObservable().Subscribe(_ =>
            {
                ClearChat();
            }).AddTo(this);

            _buttonSend.onClick.AsObservable().Subscribe(_ =>
            {
                string message = _input.text.Trim();
                if (!string.IsNullOrEmpty(message))
                {
                    Chat.SendMessageToChat(message);
                    _input.text = string.Empty;
                }
            }).AddTo(this);
        }

        private void AddMessageToChat(NetworkPlayer sender, string message)
        {
            Color teamColor = _teamPalette.GetColor(sender.ColorType);
            string hexColor = ColorUtility.ToHtmlStringRGB(teamColor);

            if (_containerMessages.Length > 0)
            {
                _containerMessages.Append("\n");
            }

            string formattedMessage = $"<color=#{hexColor}>{sender.NickName}</color>: {message}";
            _containerMessages.Append(formattedMessage);
        }

        private void UpdateChatView()
        {
            _textChat.text = _containerMessages.ToString();
        }

        private void ClearChat()
        {
            _containerMessages.Clear();
            UpdateChatView();
        }

        public void SetStateActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}