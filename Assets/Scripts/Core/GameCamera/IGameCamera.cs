using UnityEngine;

namespace CastleFight.Core.GameCamera
{
    public interface IGameCamera
    {
        void SetFollowTarget(Transform target, Vector3 offset = default);
    }
}
