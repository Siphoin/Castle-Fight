using System.Collections;
using CastleFight.Core.HealthSystem;
using Sirenix.OdinInspector;
using UnityEngine;
namespace CastleFight.Core.ConstructionSystem
{
    [RequireComponent(typeof(HealthComponent))]
    public class BuildingConstructInstance : OwnedEntity, IBuildingConstructInstance
    {
        [SerializeField, ReadOnly] private HealthComponent _healthComponent;
        public IHealthComponent HealthComponent => _healthComponent;

        private void OnValidate()
        {
            if (_healthComponent is null)
            {
                _healthComponent = GetComponent<HealthComponent>();
            }
        }
    }
}