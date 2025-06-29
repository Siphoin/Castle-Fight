// BuildingInstance.cs
using CastleFight.Core.HealthSystem;
using CastleFight.Core.BuildingsSystem.SO;
using CastleFight.Core.BuildingsSystem.Components;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CastleFight.Core.BuildingsSystem
{
    [RequireComponent(typeof(BuildingSpawnHandler))]
    public class BuildingInstance : OwnedEntity, IBuildingInstance
    {
        [SerializeField, ReadOnly] private HealthComponent _healthComponent;
        [SerializeField] private ScriptableBuuidingEntity _stats;

        public IHealthComponent HealthComponent => _healthComponent;
        public ScriptableBuuidingEntity Stats => _stats;

        protected override void Start()
        {
            base.Start();
            if (IsOwner && !IsOwnerSeted)
            {
                _healthComponent.SetHealthData(_stats.MaxHealth);
            }
        }

        private void OnValidate()
        {
            if (!_healthComponent) _healthComponent = GetComponent<HealthComponent>();
        }
    }
}