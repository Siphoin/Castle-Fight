using System;
using System.Collections;
using CastleFight.Networking.Handlers;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CastleFight.Core.BuildingsSystem.Components
{
    public class BuildingSpawnHandler : MonoBehaviour
    {
        [SerializeField, ReadOnly] private BuildingInstance _buildingInstance;
        [SerializeField] private Transform _pointSpawn;
        private INetworkHandler _networkHandler;

        private void Start()
        {
            if (_buildingInstance.IsOwner && _buildingInstance.Stats.TrainableUnit != null)
            {
                _networkHandler = FindAnyObjectByType<NetworkHandler>();
                TickSpawn().Forget();
            }
        }

        private void OnValidate()
        {
            if (_buildingInstance is null)
            {
                _buildingInstance = GetComponent<BuildingInstance>();
            }
        }

        private async UniTask TickSpawn ()
        {
            var token = this.GetCancellationTokenOnDestroy();
            TimeSpan timeSpawn = TimeSpan.FromSeconds(_buildingInstance.Stats.TrainSpeed);
            while (true)
            {
                await UniTask.Delay(timeSpawn, cancellationToken: token);
                _networkHandler.SpawnNetworkObject(_buildingInstance.Stats.TrainableUnit.gameObject, null, true, _buildingInstance.OwnerId, _pointSpawn.position, Quaternion.identity);
            }
        }
    }
}