using UnityEngine;

namespace CastleFight.Core.Components
{
    public interface IPointSpawnWorker
    {
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public int IndexPlayer {  get; }
    }
}