using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CastleFight.Core.UnitsSystem.Components
{
    public class UnitCombatSystem : MonoBehaviour
    {
        [SerializeField, ReadOnly] private UnitInstance _unitInstance;

        private void Awake()
        {
            if (!_unitInstance)
            {
                _unitInstance = GetComponentInParent<UnitInstance>();
            }
        }

        public void Damage ()
        {
            _unitInstance.NavMesh.CurrentTarget?.Damage(10, _unitInstance);
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