using UnityEngine;

namespace CastleFight.Core.Components
{
    internal interface IPointSpawnCastle
    {
        CastleTeamType TypeTeam { get; }
        Vector3 Position { get; }
        Quaternion Rotation { get; }
    }
}