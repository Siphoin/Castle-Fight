using UnityEngine;

namespace CastleFight.Core.Components
{
    public interface IPointCameraView
    {
        CastleTeamType TeamType { get; }
        Vector3 Position { get; }
        Quaternion Rotation { get; }
    }
}