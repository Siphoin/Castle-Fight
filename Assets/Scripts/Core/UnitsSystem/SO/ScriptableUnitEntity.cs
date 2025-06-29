using CastleFight.Core.SO;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CastleFight.Core.UnitsSystem.SO
{
    [CreateAssetMenu(menuName = "Core/Units/New Unit")]
    public class ScriptableUnitEntity : ScriptableLiveEntity
    {
        [Title("Combat Stats")]
        [BoxGroup("Combat Stats")]
        [MinValue(0)]
        [SerializeField] private int _damage = 10;

        [BoxGroup("Combat Stats")]
        [MinValue(0.1f)]
        [SerializeField] private float _attackSpeed = 1f;

        [BoxGroup("Combat Stats")]
        [MinValue(0)]
        [SerializeField] private float _attackRange = 1.5f;

        [Title("Movement Stats")]
        [BoxGroup("Movement Stats")]
        [MinValue(0)]
        [SerializeField] private float _moveSpeed = 3f;

        [BoxGroup("Movement Stats")]
        [MinValue(0)]
        [SerializeField] private float _rotationSpeed = 120f;

        public int Damage => _damage;
        public float AttackSpeed => _attackSpeed;
        public float AttackRange => _attackRange;

        public float MoveSpeed => _moveSpeed;
        public float RotationSpeed => _rotationSpeed;

    }
}