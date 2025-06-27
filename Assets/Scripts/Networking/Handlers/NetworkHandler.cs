using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Zenject;
using UniRx;
using CastleFight.Networking.Configs;
using System;
using Sirenix.OdinInspector;

namespace CastleFight.Networking.Handlers
{
    [RequireComponent(typeof(UnityTransport))]
    [RequireComponent(typeof(NetworkManager))]
    public class NetworkHandler : MonoBehaviour, INetworkHandler
    {
        [Inject] private NetworkHandlerConfig _config;
        [SerializeField, ReadOnly] private NetworkManager _networkManager;

        private readonly Subject<Unit> _onConnected = new Subject<Unit>();
        private readonly Subject<Unit> _onDisconnected = new Subject<Unit>();

        public IObservable<Unit> OnConnected => _onConnected;
        public IObservable<Unit> OnDisconnected => _onDisconnected;

        public bool IsConnected => _networkManager != null &&
                                 _networkManager.IsListening;

        private void Start()
        {
            _networkManager.OnClientConnectedCallback += HandleClientConnected;
            _networkManager.OnClientDisconnectCallback += HandleClientDisconnected;
            _networkManager.OnServerStarted += HandleServerStarted;
            _networkManager.OnServerStopped += HandleServerStopped;
        }

        private void OnEnable()
        {
            transform.SetParent(null);
        }

        private void OnDisable()
        {
            if (_networkManager != null)
            {
                _networkManager.OnClientConnectedCallback -= HandleClientConnected;
                _networkManager.OnClientDisconnectCallback -= HandleClientDisconnected;
                _networkManager.OnServerStarted -= HandleServerStarted;
                _networkManager.OnServerStopped -= HandleServerStopped;
            }
        }

        private void Awake()
        {
            var transport = GetComponent<UnityTransport>();
            if (transport != null)
            {
                transport.SetConnectionData(_config.DefaultIP, _config.Port);
            }
        }

        private void HandleServerStarted()
        {
            // Хост успешно запущен
            _onConnected.OnNext(Unit.Default);
            Debug.Log("Server started - host connected");
        }

        private void HandleServerStopped(bool gracefully)
        {
            // Хост остановлен
            _onDisconnected.OnNext(Unit.Default);
            Debug.Log("Server stopped - host disconnected");
        }

        private void HandleClientConnected(ulong clientId)
        {
            if (clientId == _networkManager.LocalClientId)
            {
                _onConnected.OnNext(Unit.Default);
                Debug.Log("Client connected");
            }
        }

        private void HandleClientDisconnected(ulong clientId)
        {
            if (clientId == _networkManager.LocalClientId)
            {
                _onDisconnected.OnNext(Unit.Default);
                Debug.Log("Client disconnected");
            }
        }

        public void StartServer()
        {
            if (IsConnected)
            {
                Debug.Log("Already hosting or connected!");
                return;
            }

            _networkManager.StartHost();
            Debug.Log($"P2P Host started on {_config.DefaultIP}:{_config.Port}");
        }

        public void StartClient(string targetIP)
        {
            if (IsConnected)
            {
                Debug.Log("Already connected!");
                return;
            }

            string ip = string.IsNullOrEmpty(targetIP) ? _config.DefaultIP : targetIP;

            var transport = GetComponent<UnityTransport>();
            if (transport != null)
            {
                transport.SetConnectionData(ip, _config.Port);
            }

            _networkManager.StartClient();
            Debug.Log($"Client connecting to {ip}:{_config.Port}...");
        }

        public void Disconnect()
        {
            if (!IsConnected)
            {
                Debug.LogError("Not connected!");
                return;
            }

            _networkManager.Shutdown();
            Debug.Log("Disconnected from the network.");
        }

        private void OnValidate()
        {
            if (_networkManager is null)
            {
                _networkManager = GetComponent<NetworkManager>();
            }

        }
    }
}