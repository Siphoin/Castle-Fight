using CastleFight.Core;
using CastleFight.Core.Configs;
using UnityEngine;
using UniRx;
using CastleFight.Core.UnitsSystem;
namespace Core.Components
{
    public class MaterialOwnerHandler : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private MaterialOwnerHandlerConfig _config;
        private IOwnerable _ownerable;

        private void Awake()
        {
            _ownerable = GetComponentInParent<IOwnerable>();
            _ownerable.OnPlayerOwnerChanged.Subscribe(owner =>
            {

                UpdateMaterial();

            }).AddTo(this);
        }

        private void Start()
        {
            UpdateMaterial();
        }

        private void UpdateMaterial()
        {
            int index = (int)_ownerable.OwnerId;

            Material material = _ownerable is UnitInstance unitInstance ? _config.GetUnitMaterial(index) : _config.GetBuildingMaterial(index);

            _meshRenderer.material = material;
        }
    }
}