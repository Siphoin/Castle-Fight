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
        private readonly Subject<GameObject> _onObjectDestroyed = new();

        public IObservable<(ulong requestId, GameObject spawnedObject)> OnObjectSpawned => _onObjectSpawned;
        public IObservable<GameObject> OnObjectDestroyed => _onObjectDestroyed;

        private void Start()
        {
            _networkManager = NetworkManager.Singleton;
        }

        public GameObject SpawnNetworkObject(
            GameObject prefab,
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
                RegisterCallback(requestId, callback);
                RequestSpawnServerRpc(
                    requestId,
                    prefab.name,
                    position,
                    rotation,
                    spawnWithOwnership,
                    ownerClientId
                );
                return null;
            }
        }

        public void DestroyNetworkObject(GameObject gameObject, Action callback = null)
        {
            if (!IsSpawned || gameObject == null)
            {
                callback?.Invoke();
                return;
            }

            var networkObject = gameObject.GetComponent<NetworkObject>();
            if (networkObject == null)
            {
                Debug.LogError("GameObject doesn't have NetworkObject component!");
                callback?.Invoke();
                return;
            }

            if (IsServer)
            {
                DestroyObjectDirect(gameObject);
                callback?.Invoke();
                NotifyClientDestroyedClientRpc(networkObject.NetworkObjectId);
            }
            else
            {
                RegisterDestroyCallback(networkObject.NetworkObjectId, callback);
                RequestDestroyServerRpc(networkObject.NetworkObjectId);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestDestroyServerRpc(ulong networkObjectId, ServerRpcParams rpcParams = default)
        {
            if (_networkManager.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var netObj))
            {
                DestroyObjectDirect(netObj.gameObject);
                NotifyClientDestroyedClientRpc(networkObjectId);
            }
        }

        [ClientRpc]
        private void NotifyClientDestroyedClientRpc(ulong destroyedObjectId)
        {
            if (_networkManager.SpawnManager.SpawnedObjects.TryGetValue(destroyedObjectId, out var netObj))
            {
                _onObjectDestroyed.OnNext(netObj.gameObject);
            }
        }

        private void DestroyObjectDirect(GameObject gameObject)
        {
            var networkObject = gameObject.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Despawn();
            }
            Destroy(gameObject);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestSpawnServerRpc(
            ulong requestId,
            string prefabName,
            Vector3 position,
            Quaternion rotation,
            bool spawnWithOwnership,
            ulong ownerClientId,
            ServerRpcParams rpcParams = default)
        {
            var prefab = FindPrefabByName(prefabName);
            if (prefab == null)
            {
                Debug.LogError($"Prefab with name '{prefabName}' not found in NetworkManager's prefab list!");
                return;
            }

            var go = SpawnObjectDirect(prefab, position, rotation, spawnWithOwnership, ownerClientId);
            NotifyClientSpawnedClientRpc(requestId, go.GetComponent<NetworkObject>().NetworkObjectId);
        }

        [ClientRpc]
        private void NotifyClientSpawnedClientRpc(ulong requestId, ulong spawnedObjectId)
        {
            if (_networkManager.SpawnManager.SpawnedObjects.TryGetValue(spawnedObjectId, out var netObj))
            {
                _onObjectSpawned.OnNext((requestId, netObj.gameObject));
            }
            else
            {
                Debug.LogError($"NetworkObject with ID {spawnedObjectId} not found!");
            }
        }

        private GameObject SpawnObjectDirect(
            GameObject prefab,
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

        private ulong GenerateRequestId()
        {
            ulong time = (ulong)DateTime.Now.Ticks;
            return time % ulong.MaxValue;
        }

        private void RegisterCallback(ulong requestId, Action<GameObject> callback)
        {
            _onObjectSpawned
                .Where(x => x.Item1 == requestId)
                .Take(1)
                .Subscribe(x => callback?.Invoke(x.Item2));
        }

        private void RegisterDestroyCallback(ulong networkObjectId, Action callback)
        {
            _onObjectDestroyed
                .Where(x => x.GetComponent<NetworkObject>().NetworkObjectId == networkObjectId)
                .Take(1)
                .Subscribe(_ => callback?.Invoke());
        }

        private GameObject FindPrefabByName(string prefabName)
        {
            foreach (var prefab in _networkManager.NetworkConfig.Prefabs.Prefabs)
            {
                if (prefab.Prefab.name == prefabName)
                    return prefab.Prefab;
            }
            return null;
        }
    }
}