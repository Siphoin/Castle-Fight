using Assets.Scripts.Core.Components;
using CastleFight.Core.Graphic;
using CastleFight.Core.Handlers;
using CastleFight.Core.HealthSystem;
using CastleFight.Core.UnitsSystem.Components;
using CastleFight.Core.UnitsSystem.Configs;
using CastleFight.Core.UnitsSystem.SO;
using Sirenix.OdinInspector;
using UniRx;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using Zenject;

namespace CastleFight.Core.UnitsSystem
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(UnitNavMesh))]
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(UnitObjectRepository))]
    [RequireComponent(typeof(ComponentDisableAfterDeadHandler))]
    [RequireComponent(typeof(SelectorHandler))]
    public class UnitInstance : OwnedEntity, IUnitInstance
    {
        [SerializeField, ReadOnly] private HealthComponent _healthComponent;
        [SerializeField, ReadOnly] private UnitNavMesh _navMesh;
        [SerializeField, ReadOnly] private UnitAnimatorHandler _unitAnimatorHandler;
        [SerializeField, ReadOnly] private UnitCombatSystem _combatSystem;
        [SerializeField, ReadOnly] private Collider _collider;
        [SerializeField, ReadOnly] private Portail _portail;
        [SerializeField, ReadOnly] private SelectorHandler _selectorHandler;
        [SerializeField] private ScriptableUnitEntity _stats;
        [Inject] private UnitGlobalConfig _globalConfig;


        public IHealthComponent HealthComponent => _healthComponent;
        public IUnitNavMesh NavMesh => _navMesh;
        public IUnitAnimatorHandler AnimatorHandler => _unitAnimatorHandler;
        public ScriptableUnitEntity Stats => _stats;

        public IUnitCombatSystem Combat => _combatSystem;

        public string Name => _stats.EntityName;

        public string DamageInfo => _stats.GetDamageInfo();

        public IPortail Portail => _portail;

        public float SelectionScale => _stats.SelectionScale;

        public bool IsSelected => _selectorHandler.IsSelect;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsMy)
            {
                _healthComponent.SetHealthData(_stats.MaxHealth);
                _healthComponent.OnCurrentHealthChanged.Subscribe(health =>
                {

                    if (health <= 0)
                    {
                        TimedDestroyHandler timedDestroyHandler = gameObject.AddComponent<TimedDestroyHandler>();
                        timedDestroyHandler.TimeDestroy = _globalConfig.TimeDestroyCorpse;
                        timedDestroyHandler.DestroyObject();
                    }

                }).AddTo(this);
            }
        }

        private void OnValidate()
        {
            if (!_healthComponent) _healthComponent = GetComponent<HealthComponent>();
            if (!_navMesh) _navMesh = GetComponent<UnitNavMesh>();
            if (!_unitAnimatorHandler) _unitAnimatorHandler = GetComponentInChildren<UnitAnimatorHandler>();
            if (!_collider) _collider = GetComponent<Collider>();
            if (!_combatSystem) _combatSystem = GetComponentInChildren<UnitCombatSystem>();
            if (!_portail) _portail = GetComponentInChildren<Portail>();
            if (!_selectorHandler) _selectorHandler = GetComponent<SelectorHandler>();
        }

        public void Disable()
        {
            _collider.enabled = false;
        }

        public void SetStateSelect(bool visible)
        {
            _selectorHandler.SetVisible(visible);
        }
    }
}