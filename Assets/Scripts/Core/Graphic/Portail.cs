using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CastleFight.Core.Graphic
{
    public class Portail : MonoBehaviour, IPortail
    {
        [Title("Portrait Settings")]
        [SerializeField] private Vector3 _portraitPosition = new Vector3(0, -0.69f, 0);
        [SerializeField] private Vector3 _portraitRotation = new Vector3(0, 172.42f, 0);
        [SerializeField] private Vector3 _scale = Vector3.one;
        [SerializeField, ReadOnly] private MeshFilter _meshFitler;

        public Vector3 PortraitPosition => _portraitPosition;
        public Vector3 PortraitRotation => _portraitRotation;

        public Vector3 Scale => _scale;

        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Portail");
            transform.localScale = _scale;
        }

        public void SetMesh (Mesh mesh)
        {
            if (_meshFitler != null)
            {
                _meshFitler.mesh = mesh;
            }
        }

        private void OnValidate()
        {
            if (_meshFitler is null)
            {
                _meshFitler = GetComponent<MeshFilter>();
            }
        }
    }
}