using CastleFight.Core.HealthSystem;
using CastleFight.Core.BuildingsSystem.SO;
using CastleFight.Core.BuildingsSystem.Components;
using Sirenix.OdinInspector;
using UnityEngine;
using CastleFight.Core.Handlers;
using CastleFight.Core.Graphic;
using CastleFight.Core.ConstructionSystem;

namespace CastleFight.Core.BuildingsSystem
{
    [RequireComponent(typeof(BuildingSpawnHandler))]
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(BuildingObjectRepository))]
    [RequireComponent(typeof(ComponentDisableAfterDeadHandler))]
    [RequireComponent(typeof(SelectorHandler))]
    [RequireComponent(typeof(BuildingConstructHandler))]
    public class BuildingInstance : OwnedEntity, IBuildingInstance
    {
        [SerializeField, ReadOnly] private HealthComponent _healthComponent;
        [SerializeField] private ScriptableBuuidingEntity _stats;
        [SerializeField] private Portail _portail;
        [SerializeField, ReadOnly] private SelectorHandler _selectorHandler;
        [SerializeField, ReadOnly] private BuildingConstructHandler _constructHandler;

        public bool IsContructed { get; internal set; } = true;
        public bool HasConstruction => _stats.BuildTime > 0;

        public IHealthComponent HealthComponent => _healthComponent;
        public ScriptableBuuidingEntity Stats => _stats;

        public string Name => _stats.EntityName;
        public string DamageInfo => _stats.GetDamageInfo();
        public IPortail Portail => _portail;
        public float SelectionScale => _stats.SelectionScale;
        public IBuildingConstructHandler ConstructHandler => _constructHandler;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsMy)
            {
                _healthComponent.SetHealthData(_stats.MaxHealth);
            }
        }

        public void SetStateSelect(bool visible)
        {
            _selectorHandler.SetVisible(visible);
        }

        public void TurnConstruct()
        {
            if (HasConstruction && IsContructed)
            {
                IsContructed = false;
                _healthComponent.TurnConstructHealth();
                _constructHandler.TurnConstruct();
            }
        }

        private void OnValidate()
        {
            if (!_healthComponent) _healthComponent = GetComponent<HealthComponent>();
            if (!_selectorHandler) _selectorHandler = GetComponent<SelectorHandler>();
            if (!_constructHandler) _constructHandler = GetComponent<BuildingConstructHandler>();
        }
    }
}