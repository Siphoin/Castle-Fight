using System;
using System.Collections;
using CastleFight.Core.HealthSystem;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace CastleFight.Core.UnitsSystem.Components
{
    public class UnitCombatSystem : MonoBehaviour, IUnitCombatSystem
    {
        [SerializeField, ReadOnly] private UnitInstance _unitInstance;
        private Subject<IHealthComponent> _onKill = new();

        public IObservable<IHealthComponent> OnKill => _onKill;

        private void Awake()
        {
            if (!_unitInstance)
            {
                _unitInstance = GetComponentInParent<UnitInstance>();
            }
        }

        public void Damage ()
        {
            if (_unitInstance.NavMesh.CurrentTarget != null)
            {
                var damage = _unitInstance.Stats.GetFinalDamage();
                var totalHealthFromDDamage = _unitInstance.NavMesh.CurrentTarget.Health - damage;
                _unitInstance.NavMesh.CurrentTarget?.Damage(damage, _unitInstance.NetworkObjectId);

                if (totalHealthFromDDamage <= 0)
                {
                    _onKill.OnNext(_unitInstance.NavMesh.CurrentTarget);
                }
            }
        }

        private void OnValidate()
        {
            if (!_unitInstance)
            {
                _unitInstance = GetComponentInParent<UnitInstance>();
            }
        }
    }
}