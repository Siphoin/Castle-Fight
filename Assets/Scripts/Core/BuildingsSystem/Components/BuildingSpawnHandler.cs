using System;
using CastleFight.Core.UnitsSystem.Factories;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace CastleFight.Core.BuildingsSystem.Components
{
    public class BuildingSpawnHandler : MonoBehaviour
    {
        [SerializeField, ReadOnly] private BuildingInstance _buildingInstance;
        [SerializeField] private Transform _pointSpawn;
        [Inject]  private IUnitFactory _unitFactory;
        private static readonly Quaternion DefaultRotation = Quaternion.Euler(0f, -190f, 0f);

        public Vector3 SpawnPoint => _pointSpawn != null ? _pointSpawn.position : Vector3.negativeInfinity;

        private void Start()
        {

            if (_buildingInstance.IsOwner && _buildingInstance.Stats.TrainableUnit != null)
            {
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
            await UniTask.WaitUntil(() => _buildingInstance.IsContructed, cancellationToken: token);
            Debug.Log("TURN SPAWN");
            while (true)
            {
                await UniTask.Delay(timeSpawn, cancellationToken: token);
                _unitFactory.Create(_buildingInstance.Stats.TrainableUnit, _pointSpawn.position, DefaultRotation);
            }

        }

    }
    }