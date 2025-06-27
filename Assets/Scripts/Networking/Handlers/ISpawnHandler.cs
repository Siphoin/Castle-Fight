using System;
using UnityEngine;

namespace CastleFight.Networking.Handlers
{
    public interface ISpawnHandler
    {
        GameObject SpawnNetworkObject(GameObject prefab,
                            Action<GameObject> callback,
                            bool spawnWithOwnership = true,
                            ulong ownerClientId = 0,
                            Vector3 position = default,
                            Quaternion rotation = default);
    }
}
