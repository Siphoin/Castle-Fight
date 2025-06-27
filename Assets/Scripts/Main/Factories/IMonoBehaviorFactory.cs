using UnityEngine;

namespace CastleFight.Main.Factories
{
    public interface IMonoBehaviorFactory
    {
    }
    public interface IMonoBehaviorFactory<TPrefab, TReturn> : IMonoBehaviorFactory where TReturn : IFactoryObject where TPrefab : MonoBehaviour
    {
        TReturn Create(TPrefab prefab, Vector3 position, Quaternion rotation, Transform? parent = null);
    }
}
