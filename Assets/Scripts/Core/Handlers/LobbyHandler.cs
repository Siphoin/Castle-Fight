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
        private NetworkVariable<ushort> _currentTicks = new();
        private bool _isRunning = false;

        private Subject<ushort> _onTick = new();
        private Subject<Unit> _onEndTick = new();
        [SerializeField] private LobbyHandlerConfig _config;
        private IPlayerListHandler _playerListHandler;

        public IObservable<ushort> OnTick => _onTick;

        public bool IsStarted => _isStarted.Value;

        public bool AllPlayersIsReady
        {
            get
            {
                return _playerListHandler.All(x => x.IsReady);
            }
        }

        public IObservable<Unit> OnEndTick => _onEndTick;

        private void Awake()
        {
            _playerListHandler = FindAnyObjectByType<PlayerListHandler>();
        }

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
            if (newValue == 0)
            {
                _onEndTick.OnNext(Unit.Default);
            }


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

            _onEndTick.OnNext(Unit.Default);


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