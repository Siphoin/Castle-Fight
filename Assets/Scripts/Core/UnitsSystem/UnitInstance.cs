using CastleFight.Core.Handlers;
using CastleFight.Core.HealthSystem;
using CastleFight.Core.UnitsSystem.Components;
using CastleFight.Core.UnitsSystem.SO;
using Sirenix.OdinInspector;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace CastleFight.Core.UnitsSystem
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(UnitNavMesh))]
    [RequireComponent(typeof(NetworkTransform))]
    [RequireComponent(typeof(UnitObjectRepository))]
    [RequireComponent(typeof(ComponentDisableAfterDeadHandler))]
    public class UnitInstance : OwnedEntity, IUnitInstance
    {
        [SerializeField, ReadOnly] private HealthComponent _healthComponent;
        [SerializeField, ReadOnly] private UnitNavMesh _navMesh;
        [SerializeField, ReadOnly] private UnitAnimatorHandler _unitAnimatorHandler;
        [SerializeField, ReadOnly] private Collider _collider;
        [SerializeField] private ScriptableUnitEntity _stats;


        public IHealthComponent HealthComponent => _healthComponent;
        public IUnitNavMesh NavMesh => _navMesh;
        public IUnitAnimatorHandler AnimatorHandler => _unitAnimatorHandler;
        public ScriptableUnitEntity Stats => _stats;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsMy)
            {
                _healthComponent.SetHealthData(_stats.MaxHealth);
            }
        }

        private void OnValidate()
        {
            if (!_healthComponent) _healthComponent = GetComponent<HealthComponent>();
            if (!_navMesh) _navMesh = GetComponent<UnitNavMesh>();
            if (!_unitAnimatorHandler) _unitAnimatorHandler = GetComponentInChildren<UnitAnimatorHandler>();
            if (!_collider) _collider = GetComponent<Collider>();
        }

        public void Disable()
        {
            _collider.enabled = false;
        }
    }
}