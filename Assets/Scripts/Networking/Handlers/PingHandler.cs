using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UniRx;
using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace CastleFight.Networking.Handlers
{
    public class PingHandler : NetworkBehaviour
    {
        private UnityTransport _transport;
        private NetworkManager _networkManager;

        private readonly Subject<int> _onPingChanged = new Subject<int>();
        public IObservable<int> OnPingChanged => _onPingChanged;

        [SerializeField] private float _pingCheckInterval = 1.0f;
        private Stopwatch _pingStopwatch = new Stopwatch();
        private bool _rpcReceived;
        private int _lastPing = -1;

        private void Awake()
        {
            _networkManager = NetworkManager.Singleton;
            _transport = FindAnyObjectByType<UnityTransport>();
            CheckPingLoop().Forget();
        }

        private async UniTask CheckPingLoop()
        {
            while (true)
            {
                if (IsClient)
                {
                    await MeasurePingAsync();
                }

                await UniTask.Delay(TimeSpan.FromSeconds(_pingCheckInterval), ignoreTimeScale: true);
            }
        }

        private async UniTask MeasurePingAsync()
        {
            _pingStopwatch.Restart();
            _rpcReceived = false;
            PingServerRpc();

            try
            {
                // Ожидаем получения Pong с таймаутом 5 секунд
                await UniTask.WaitUntil(() => _rpcReceived)
                    .Timeout(TimeSpan.FromSeconds(5));

                _pingStopwatch.Stop();
                int pingMs = (int)_pingStopwatch.ElapsedMilliseconds / 2;
                _onPingChanged.OnNext(pingMs);
                Debug.Log($"Ping: {pingMs}ms");
            }
            catch (TimeoutException)
            {
                Debug.LogWarning("Ping measurement timed out");
                _onPingChanged.OnNext(-1);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void PingServerRpc(ServerRpcParams rpcParams = default)
        {
            var clientId = rpcParams.Receive.SenderClientId;
            PongClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } } });
        }

        [ClientRpc]
        private void PongClientRpc(ClientRpcParams rpcParams = default)
        {
            _rpcReceived = true;
        }
    }
}