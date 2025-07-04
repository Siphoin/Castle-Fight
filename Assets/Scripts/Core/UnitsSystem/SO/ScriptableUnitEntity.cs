﻿using CastleFight.Core.SO;
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
        [MinValue(0)]
        [SerializeField] private float _damageSpread = 0.5f;

        [BoxGroup("Combat Stats")]
        [ShowInInspector, ReadOnly]
        [LabelText("Final Damage (Debug)")]
        private string FinalDamageDebug => $"{_damage - _damageSpread:0.0} — {_damage + _damageSpread:0.0}";

        [BoxGroup("Combat Stats")]
        [MinValue(0.1f)]
        [SerializeField] private float _attackSpeed = 1f;

        [BoxGroup("Combat Stats")]
        [MinValue(0)]
        [SerializeField] private float _attackRange = 1.5f;

        [BoxGroup("Combat Stats")]
        [MinValue(0)]
        [SerializeField] private float _initialAttackDelayRandomness = 0.1f;

        [BoxGroup("Combat Stats")]
        [MinValue(0)]
        [SerializeField] private float _attackSpeedRandomness = 0.05f;

        [Title("Movement Stats")]
        [BoxGroup("Movement Stats")]
        [MinValue(0)]
        [SerializeField] private float _moveSpeed = 3f;

        public float DamageSpread => _damageSpread;
        public float AttackSpeed => _attackSpeed;
        public float AttackRange => _attackRange;
        public float MoveSpeed => _moveSpeed;

        public float GetFinalDamage()
        {
            float randomSpread = Random.Range(-_damageSpread, _damageSpread);
            return _damage + randomSpread;
        }

        public float GetInitialAttackDelay() => Random.Range(0, _initialAttackDelayRandomness);
        public float GetFinalAttackSpeed() => _attackSpeed * (1 + Random.Range(-_attackSpeedRandomness, _attackSpeedRandomness));

        public string GetDamageInfoForEditor() => FinalDamageDebug;

        public string GetDamageInfo() => $"{(int)(_damage - _damageSpread)} — {(int)(_damage + _damageSpread)}";
    }
}