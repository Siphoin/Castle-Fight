using CastleFight.Networking.Handlers;
using UnityEngine;
using Zenject;

namespace CastleFight.Core.UnitsSystem.Factories
{
    public class UnitFactory : IUnitFactory
    {
        [Inject] private INetworkHandler _network;
        public IUnitInstance Create(UnitInstance prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var newUnit = _network.SpawnNetworkObject(prefab.gameObject, null, true, _network.LocalClientId, position, rotation);

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
    }
}
