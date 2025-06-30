using System;
using System.Linq;
using CastleFight.Core.Configs;
using CastleFight.Networking.Handlers;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Netcode;
using UnityEngine;
namespace CastleFight.Core.Handlers
{
    public class LobbyHandler : NetworkBehaviour, ILobbyHandler
    {
        private NetworkVariable<bool> _isStarted = new(false);
        private NetworkVariable<bool> _isTickingTimer = new(false);
        private NetworkVariable<ushort> _currentTicks = new();
        private bool _isRunning = false;

        private Subject<ushort> _onTick = new();
        private Subject<Unit> _onStartTick = new();
        private Subject<Unit> _onEndTick = new();
        [SerializeField] private LobbyHandlerConfig _config;
        private IPlayerListHandler _playerListHandler;

        public IObservable<ushort> OnTick => _onTick;

        public bool IsStarted => _isStarted.Value;
        public bool IsTickingTimer => _isTickingTimer.Value;

        public bool AllPlayersIsReady
        {
            get
            {
                return _playerListHandler.All(x => x.IsReady);
            }
        }

        public IObservable<Unit> OnEndTick => _onEndTick;
        public IObservable<Unit> OnStartTick => _onStartTick;

        public ushort CurrentTicks => _currentTicks.Value;

        private void Awake()
        {
            _playerListHandler = FindAnyObjectByType<PlayerListHandler>();
        }

        private void Start()
        {
            if (!IsHost)
            {
                _currentTicks.OnValueChanged += OnTickChanged;
            }
        }

        private void OnTickChanged(ushort previousValue, ushort newValue)
        {
            if (newValue == _config.TimeToStart)
            {
                _onStartTick.OnNext(Unit.Default);
            }
            _onTick.OnNext(newValue);
            if (newValue == 0)
            {
                _onEndTick.OnNext(Unit.Default);
            }

#if UNITY_EDITOR
            Debug.Log($"{nameof(LobbyHandler)}: tick: {_currentTicks.Value}...");
#endif


        }

        public void Turn ()
        {
            if (IsHost && !_isRunning && !_isStarted.Value)
            {
                _isRunning = true;
                Tick().Forget();
            }
        }

        private async UniTask Tick ()
        {
            _currentTicks.Value = _config.TimeToStart;
            _isTickingTimer.Value = true;
            _onStartTick.OnNext(Unit.Default);
            var token = this.GetCancellationTokenOnDestroy();
            while (_currentTicks.Value > 0)
            {
                await UniTask.Delay(1000, true, cancellationToken:  token);
                _currentTicks.Value--;
                _onTick.OnNext(_currentTicks.Value);

#if UNITY_EDITOR
                Debug.Log($"{nameof(LobbyHandler)}: tick: {_currentTicks.Value}...");
#endif
            }

            _onEndTick.OnNext(Unit.Default);
            _isStarted.Value = true;
            _isRunning = false;
            _isTickingTimer.Value = false;

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