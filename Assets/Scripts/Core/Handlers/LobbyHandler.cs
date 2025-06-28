using System;
using CastleFight.Core.Configs;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Netcode;
using UnityEngine;
namespace CastleFight.Core.Handlers
{
    public class LobbyHandler : NetworkBehaviour, ILobbyHandler
    {
        private NetworkVariable<bool> _isStarted = new(false);
        private NetworkVariable<ushort> _currentTicks = new();
        private bool _isRunning = false;

        private Subject<ushort> _onTick = new();
        [SerializeField] private LobbyHandlerConfig _config;

        public IObservable<ushort> OnTick => _onTick;

        public bool IsStarted => _isStarted.Value;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            if (!IsHost)
            {
                _currentTicks.OnValueChanged += OnTickChanged;
            }
        }

        private void OnTickChanged(ushort previousValue, ushort newValue)
        {
            _onTick.OnNext(newValue);
        }

        public void Turn ()
        {
            if (IsHost && !_isRunning)
            {
                _isRunning = true;
                Tick().Forget();
            }
        }

        private async UniTask Tick ()
        {
            _currentTicks.Value = _config.TimeToStart;
            var token = this.GetCancellationTokenOnDestroy();
            while (_currentTicks.Value > 0)
            {
                await UniTask.Delay(1000, true, cancellationToken:  token);
                _currentTicks.Value--;
                _onTick.OnNext(_currentTicks.Value);
            }

            
        }

        private void OnDisable()
        {
            if (!IsHost)
            {
                _currentTicks.OnValueChanged -= OnTickChanged;
            }
        }
    }
}