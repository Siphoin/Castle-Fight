using UnityEngine;

namespace CastleFight.Core.Components
{
    public class PointSpawnCastle : MonoBehaviour, IPointSpawnCastle
    {
        [SerializeField] private CastleTeamType _typeTeam;

        public CastleTeamType TypeTeam => _typeTeam;
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;
    }

    public enum CastleTeamType
    {
        Red,
        Blue,
    }
}