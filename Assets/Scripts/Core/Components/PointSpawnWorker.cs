using System.Collections;
using UnityEngine;

namespace CastleFight.Core.Components
{
    [RequireComponent(typeof(PointSpawnWorkerRepositoryRegistrer))]
    public class PointSpawnWorker : MonoBehaviour, IPointSpawnWorker
    {
        [SerializeField] private int _indexPlayer;
        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;

        public int IndexPlayer => _indexPlayer;

    }
}