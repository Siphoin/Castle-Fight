using System;
using UnityEngine;

namespace CastleFight.Networking.Handlers
{
    public interface INetworkObjectSpawnHandler : ISpawnHandler
    {
        IObservable<(ulong requestId, GameObject spawnedObject)> OnObjectSpawned {  get; }
    }
}