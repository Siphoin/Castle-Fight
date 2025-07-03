using System;
using System.Threading.Tasks;
using CastleFight.Core.Handlers;
using Cysharp.Threading.Tasks;
using UniRx;

namespace CastleFight.Core.UI
{
    public class TimerStartGameView : UIText
    {
        private ILobbyHandler _lobbyHandler;

        private async void Awake()
        {
            Component.text = string.Empty;
            await UniTask.WaitUntil(() => FindLobbyHandler(), cancellationToken: this.GetCancellationTokenOnDestroy());
            _lobbyHandler = FindAnyObjectByType<LobbyHandler>();
            _lobbyHandler.OnStartTick.Subscribe(_ =>
            {
                UpdateTick();

            }).AddTo(this);

            _lobbyHandler.OnTick.Subscribe(_ =>
            {
                UpdateTick();

            }).AddTo(this);

            _lobbyHandler.OnEndTick.Subscribe(_ =>
            {
                Component.text = string.Empty;

            }).AddTo(this);
        }

        private bool FindLobbyHandler()
        {
            _lobbyHandler = FindAnyObjectByType<LobbyHandler>();

            return _lobbyHandler != null;
        }

        private void UpdateTick ()
        {
            Component.text = $"Game start at {_lobbyHandler.CurrentTicks} seconds...";
        }
    }
}