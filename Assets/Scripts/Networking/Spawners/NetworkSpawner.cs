using System;
using CastleFight.Networking.Handlers;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace CastleFight.Networking.Spawners
{
    public abstract class NetworkSpawner<T> : MonoBehaviour where T : NetworkBehaviour
    {
        [Inject] private INetworkHandler _network;
        [SerializeField] private T _prefab;
        private Subject<T> _onSpawn = new();

        public IObservable<T> OnSpawn => _onSpawn;

        private void Awake()
        {
            if (_network.IsHost)
            {
                _network.SpawnNetworkObject(_prefab.gameObject, OnSpawnCallback);
            }
        }

        private void OnSpawnCallback(GameObject result)
        {
            T component = result.GetComponent<T>();
            _onSpawn.OnNext(component);
        }
    }
}