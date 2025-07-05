using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CastleFight.Core.ConstructionSystem.Views
{
    public class BuildingModelView : MonoBehaviour, IBuildingModelView
    {
        [SerializeField, ReadOnly] private MeshFilter _meshFilter;
        [SerializeField, ReadOnly] private Renderer _renderer;
        public Mesh Mesh => _meshFilter.sharedMesh;

        public Vector3 Scale => transform.localScale;
        public Material Material => _renderer.sharedMaterial;

        private void OnValidate()
        {
            if (!_meshFilter) _meshFilter = GetComponent<MeshFilter>();
            if (!_renderer) _renderer = GetComponent<Renderer>();
        }
    }
}