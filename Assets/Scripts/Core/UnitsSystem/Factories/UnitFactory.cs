using System;
using CastleFight.Networking.Handlers;
using UniRx;
using UnityEngine;
using Zenject;

namespace CastleFight.Core.UnitsSystem.Factories
{
    public class UnitFactory : IUnitFactory
    {
        [Inject] private INetworkHandler _network;
        private Subject<IUnitInstance> _onSpawn = new();

        public IObservable<IUnitInstance> OnSpawn => _onSpawn;


        public IUnitInstance Create(UnitInstance prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var newUnit = _network.SpawnNetworkObject(prefab.gameObject, OnSpawnUnit, true, _network.LocalClientId, position, rotation);
            

            if (newUnit != null)
            {
                if (parent != null)
                {
                    newUnit.transform.SetParent(parent, false);
                }

                return newUnit.GetComponent<UnitInstance>();
            }

            return null;
        }

        private void OnSpawnUnit(GameObject unit)
        {
            _onSpawn.OnNext(unit.GetComponent<UnitInstance>());
            Debug.Log(nameof(OnSpawnUnit));
        }
    }
}
