using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UniRx;
using Cysharp.Threading.Tasks;
using System;

namespace CastleFight.Networking.Handlers
{
    public class PingHandler : NetworkBehaviour
    {
        private UnityTransport _transport;
        private NetworkManager _networkManager;

        private readonly Subject<int> _onPingChanged = new Subject<int>();
        public IObservable<int> OnPingChanged => _onPingChanged;

        [SerializeField] private float _pingCheckInterval = 1.0f;

        private int _lastPing = -1;
        private bool _running;

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
                    var pingMs = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(NetworkManager.Singleton.NetworkConfig.NetworkTransport.ServerClientId);

                     Debug.Log("Measured Ping: " + pingMs);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(_pingCheckInterval), ignoreTimeScale: true);
            }
        }
    }
}
