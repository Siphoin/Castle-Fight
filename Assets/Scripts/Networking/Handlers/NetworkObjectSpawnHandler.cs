using System;
using Unity.Netcode;
using UnityEngine;
using UniRx;

namespace CastleFight.Networking.Handlers
{
    internal class NetworkObjectSpawnHandler : NetworkBehaviour, INetworkObjectSpawnHandler
    {
        private NetworkManager _networkManager;
        private readonly Subject<(ulong, GameObject)> _onObjectSpawned = new();

        public IObservable<(ulong requestId, GameObject spawnedObject)> OnObjectSpawned => _onObjectSpawned;

        private void Start()
        {
            _networkManager = NetworkManager.Singleton;
        }

        public GameObject SpawnNetworkObject(GameObject prefab,
                                    Action<GameObject> callback,
                                   
                                    bool spawnWithOwnership = true,
                                    ulong ownerClientId = 0,
                                     Vector3 position = default,
                                    Quaternion rotation = default)
        {
            if (!IsSpawned || prefab == null)
            {
                callback?.Invoke(null);
                return null;
            }

            ulong requestId = GenerateRequestId();

            if (IsServer)
            {
                var go = SpawnObjectDirect(prefab, position, rotation, spawnWithOwnership, ownerClientId);
                callback?.Invoke(go);

                NotifyClientSpawnedClientRpc(requestId, go.GetComponent<NetworkObject>().NetworkObjectId);
                return go;
            }
            else
            {
                // Сохраняем callback для этого запроса
                RegisterCallback(requestId, callback);

                RequestSpawnServerRpc(
                    requestId,
                    GetNetworkPrefabHash(prefab),
                    position,
                    rotation,
                    spawnWithOwnership,
                    ownerClientId
                );

                return null;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestSpawnServerRpc(ulong requestId,
                                         ulong prefabHash,
                                         Vector3 position,
                                         Quaternion rotation,
                                         bool spawnWithOwnership,
                                         ulong ownerClientId,
                                         ServerRpcParams rpcParams = default)
        {
            var prefab = FindPrefabByHash(prefabHash);
            if (prefab == null) return;

            var go = SpawnObjectDirect(prefab, position, rotation, spawnWithOwnership, ownerClientId);
            NotifyClientSpawnedClientRpc(requestId, go.GetComponent<NetworkObject>().NetworkObjectId);
        }

        [ClientRpc]
        private void NotifyClientSpawnedClientRpc(ulong requestId, ulong spawnedObjectId)
        {
            var netObj = _networkManager.SpawnManager.SpawnedObjects[spawnedObjectId];
            _onObjectSpawned.OnNext((requestId, netObj.gameObject));
        }

        private GameObject SpawnObjectDirect(GameObject prefab,
                                          Vector3 position,
                                          Quaternion rotation,
                                          bool withOwnership,
                                          ulong clientId)
        {
            var go = Instantiate(prefab, position, rotation);
            var netObj = go.GetComponent<NetworkObject>();

            if (withOwnership)
                netObj.SpawnWithOwnership(clientId);
            else
                netObj.Spawn();

            return go;
        }

        // Генерация уникального ID для запроса
        private ulong GenerateRequestId()
        {
            ulong time = (ulong)DateTime.Now.Ticks;
            return time % ulong.MaxValue;
        }

        // Регистрация callback'ов (упрощенная версия)
        private void RegisterCallback(ulong requestId, Action<GameObject> callback)
        {
            _onObjectSpawned
                .Where(x => x.Item1 == requestId)
                .Take(1)
                .Subscribe(x => callback?.Invoke(x.Item2));
        }

        private ulong GetNetworkPrefabHash(GameObject prefab)
        {
            var netObj = prefab?.GetComponent<NetworkObject>();
            return netObj?.NetworkObjectId ?? 0;
        }

        private GameObject FindPrefabByHash(ulong hash)
        {
            foreach (var prefab in _networkManager.NetworkConfig.Prefabs.Prefabs)
            {
                var netObj = prefab.Prefab.GetComponent<NetworkObject>();
                if (netObj != null && netObj.NetworkObjectId == hash)
                    return prefab.Prefab;
            }
            return null;
        }
    }
}