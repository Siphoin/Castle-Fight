using System;
using CastleFight.Networking.Handlers;
using UniRx;
using UnityEngine;
using Zenject;

namespace CastleFight.Core.BuildingsSystem.Factories
{
    public class BuildingFactory : IBuildingFactory
    {
        [Inject] private INetworkHandler _network;
        private Subject<IBuildingInstance> _onSpawn = new();

        public IObservable<IBuildingInstance> OnSpawn => throw new NotImplementedException();

        public IBuildingInstance Create(BuildingInstance prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var newBuilding = _network.SpawnNetworkObject(prefab.gameObject, OnSpawnBuilding, true, _network.LocalClientId, position, rotation);

            if (newBuilding != null)
            {
                if (parent != null)
                {
                    newBuilding.transform.SetParent(parent, false);
                }

                return newBuilding.GetComponent<BuildingInstance>();
            }

            return null;
        }

        private void OnSpawnBuilding(GameObject building)
        {
            _onSpawn.OnNext(building.GetComponent<BuildingInstance>());
        }
    }
}
