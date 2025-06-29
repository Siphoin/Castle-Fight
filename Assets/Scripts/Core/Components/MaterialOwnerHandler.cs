using CastleFight.Core;
using CastleFight.Core.Configs;
using UnityEngine;
using UniRx;
using CastleFight.Core.UnitsSystem;

namespace Core.Components
{
    public class MaterialOwnerHandler : MonoBehaviour
    {
        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private MaterialOwnerHandlerConfig _config;
        private IOwnerable _ownerable;

        private void Awake()
        {
            _ownerable = GetComponentInParent<IOwnerable>();
            _ownerable.OnPlayerOwnerChanged.Subscribe(owner =>
            {
                UpdateMaterials();
            }).AddTo(this);
        }

        private void Start()
        {
            UpdateMaterials();
        }

        private void UpdateMaterials()
        {

            int index = (int)_ownerable.OwnerId;
            Material material = _ownerable is UnitInstance unitInstance
                ? _config.GetUnitMaterial(index)
                : _config.GetBuildingMaterial(index);

            foreach (var renderer in _renderers)
            {
                if (renderer != null)
                {
                    renderer.material = material;
                }
            }
        }
    }
}