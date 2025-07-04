using System;
using System.Collections;
using CastleFight.Core.HealthSystem;
using Sirenix.OdinInspector;
using UnityEngine;
namespace CastleFight.Core.ConstructionSystem
{
    [RequireComponent(typeof(HealthComponent))]
    public class BuildingConstructHandler : OwnedEntity, IBuildingConstructHandler
    {
        [SerializeField, ReadOnly] private HealthComponent _healthComponent;
        public IHealthComponent HealthComponent => _healthComponent;

        public void TurnConstruct()
        {
            Debug.Log(nameof(TurnConstruct));
        }

        private void OnValidate()
        {
            if (_healthComponent is null)
            {
                _healthComponent = GetComponent<HealthComponent>();
            }
        }
    }
}