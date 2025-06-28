using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Zenject;
using UniRx;
using CastleFight.Networking.Configs;
using System;
using Sirenix.OdinInspector;
using CastleFight.Networking.Models;

namespace CastleFight.Networking.Handlers
{
    [RequireComponent(typeof(UnityTransport))]
    [RequireComponent(typeof(NetworkManager))]
    public class NetworkHandler : MonoBehaviour, INetworkHandler
    {
        [Inject] private NetworkHandlerConfig _config;
        [SerializeField, ReadOnly] private NetworkManager _networkManager;
        [SerializeField, ReadOnly] private PlayerListHandler _playerListHandler;
        [SerializeField, ReadOnly] private NetworkObjectSpawnHandler _objectSpawnHandler;

        private readonly Subject<Unit> _onConnected = new Subject<Unit>();
        private readonly Subject<Unit> _onDisconnected = new Subject<Unit>();

        public IObservable<Unit> OnConnected => _onConnected;
        public IObservable<Unit> OnDisconnected => _onDisconnected;

        public bool IsConnected => _networkManager != null &&
                                 _networkManager.IsListening;

        public bool IsHost => _networkManager != null && _networkManager.IsHost;

        public ulong LocalClientId => _networkManager.LocalClientId;
        public NetworkClient LocalClient => _networkManager.LocalClient;

        public static string SetedNickName {  get; private set; } = string.Empty;

        public IPlayerListHandler Players
        {
            get
            {
                if (_playerListHandler is null)
                {
                    _playerListHandler = FindAnyObjectByType<PlayerListHandler>();
                }

                return _playerListHandler;
            }
        }

       private INetworkObjectSpawnHandler ObjectSpawner
        {
            get
            {
                if (_objectSpawnHandler is null)
                {
                    _objectSpawnHandler = FindAnyObjectByType<NetworkObjectSpawnHandler>();
                }
                
                return _objectSpawnHandler;
            }
        }

        private void Start()
        {
            _networkManager.OnClientConnectedCallback += HandleClientConnected;
            _networkManager.OnClientDisconnectCallback += HandleClientDisconnected;
            _networkManager.OnServerStarted += HandleServerStarted;
            _networkManager.OnServerStopped += HandleServerStopped;
            _networkManager.LogLevel = _config.LogLevel;
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

                if (IsHost && _objectSpawnHandler is null)
                {
                    CreateSpawnHandler();
                    CreateOtherHandlers();
                }
            }
        }

        private void CreateSpawnHandler()
        {
            var go = Instantiate(_config.SpawnHandlerPrefab);
            var netObj = go.GetComponent<NetworkObject>();

            if (netObj == null)
            {
                Destroy(go);;
            }

                netObj.SpawnWithOwnership(0);

            _objectSpawnHandler = go.GetComponent<NetworkObjectSpawnHandler>();
        }

        private void CreateOtherHandlers()
        {
            foreach (var item in _config.HandlersPrefabs)
            {
                var go = Instantiate(item);
                var netObj = go.GetComponent<NetworkObject>();

                if (netObj == null)
                {
                    Destroy(go);
                    continue;
                }

                netObj.SpawnWithOwnership(0, false);

                // Если это PlayerListHandler, добавляем хост в список
                if (go.TryGetComponent(out PlayerListHandler playerList))
                {
                    _playerListHandler = playerList;
                }
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

        public void StartServer(string nickName = null)
        {
            if (IsConnected)
            {
                Debug.Log("Already hosting or connected!");
                return;
            }
            SetedNickName = nickName;
            _networkManager.StartHost();
            Debug.Log($"P2P Host started on {_config.DefaultIP}:{_config.Port}");
        }

        public void StartClient(string targetIP, string nickName = null)
        {
            if (IsConnected)
            {
                Debug.Log("Already connected!");
                return;
            }

            string ip = string.IsNullOrEmpty(targetIP) ? _config.DefaultIP : targetIP;
            SetedNickName = nickName;

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
            SetedNickName = string.Empty;
            Debug.Log("Disconnected from the network.");
        }

        private void OnValidate()
        {
            if (_networkManager is null)
            {
                _networkManager = GetComponent<NetworkManager>();
            }

        }

        public GameObject SpawnNetworkObject(GameObject prefab,
                                    Action<GameObject> callback,

                                    bool spawnWithOwnership = true,
                                    ulong ownerClientId = 0,
                                     Vector3 position = default,
                                    Quaternion rotation = default)
        {

            return ObjectSpawner.SpawnNetworkObject(
                prefab,
                callback,
                spawnWithOwnership,
                ownerClientId,
                position,
                rotation
            );
        }
    }
}