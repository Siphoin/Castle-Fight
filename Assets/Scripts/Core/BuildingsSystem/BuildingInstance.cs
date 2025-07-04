// BuildingInstance.cs
using CastleFight.Core.HealthSystem;
using CastleFight.Core.BuildingsSystem.SO;
using CastleFight.Core.BuildingsSystem.Components;
using Sirenix.OdinInspector;
using UnityEngine;
using CastleFight.Core.Handlers;
using CastleFight.Core.Graphic;

namespace CastleFight.Core.BuildingsSystem
{
    [RequireComponent(typeof(BuildingSpawnHandler))]
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(BuildingObjectRepository))]
    [RequireComponent(typeof(ComponentDisableAfterDeadHandler))]
    [RequireComponent(typeof(SelectorHandler))]
    public class BuildingInstance : OwnedEntity, IBuildingInstance
    {
        [SerializeField, ReadOnly] private HealthComponent _healthComponent;
        [SerializeField] private ScriptableBuuidingEntity _stats;
        [SerializeField] private Portail _portail;
        [SerializeField, ReadOnly]   private SelectorHandler _selectorHandler;

        public IHealthComponent HealthComponent => _healthComponent;
        public ScriptableBuuidingEntity Stats => _stats;

        public string Name => _stats.EntityName;

        public string DamageInfo => _stats.GetDamageInfo();

        public IPortail Portail => _portail;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsOwner && !IsOwnerSeted)
            {
                _healthComponent.SetHealthData(_stats.MaxHealth);
            }
        }

        public void SetStateSelect(bool visible)
        {
            _selectorHandler.SetVisible(visible);
        }

        private void OnValidate()
        {
            if (!_healthComponent) _healthComponent = GetComponent<HealthComponent>();
            if (!_selectorHandler) _selectorHandler = GetComponent<SelectorHandler>();
        }
    }
}