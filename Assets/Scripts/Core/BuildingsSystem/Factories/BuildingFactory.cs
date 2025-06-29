using CastleFight.Networking.Handlers;
using UnityEngine;
using Zenject;

namespace CastleFight.Core.BuildingsSystem.Factories
{
    public class BuildingFactory : IBuildingFactory
    {
        [Inject] private INetworkHandler _network;
        public IBuildingInstance Create(BuildingInstance prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var newBuilding = _network.SpawnNetworkObject(prefab.gameObject, null, true, _network.LocalClientId, position, rotation);

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
    }
}
