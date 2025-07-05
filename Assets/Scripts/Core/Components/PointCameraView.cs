using UnityEngine;

namespace CastleFight.Core.Components
{
    public class PointCameraView : MonoBehaviour, IPointCameraView
    {
        [SerializeField] private CastleTeamType _teamType;

        public CastleTeamType TeamType => _teamType;
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;
    }
}