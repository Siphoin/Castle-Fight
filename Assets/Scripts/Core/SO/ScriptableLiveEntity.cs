using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CastleFight.Core.SO
{
    public abstract class ScriptableLiveEntity : ScriptableObject
    {
        [Title("Basic Info")]
        [PreviewField(50, ObjectFieldAlignment.Left)]
        [SerializeField] private Sprite _icon;

        [BoxGroup("Basic Info")]
        [SerializeField] private string _entityName;

        [BoxGroup("Basic Info")]
        [TextArea(3, 10)]
        [SerializeField] private string _description;

        [Title("Health Settings")]
        [MinValue(1)]
        [SerializeField] private int _maxHealth = 100;

        [Title("Gold Bounty Settings")]
        [BoxGroup("Gold Bounty Settings")]
        [MinValue(0)]
        [SerializeField] private int _baseGoldBounty = 25;

        [BoxGroup("Gold Bounty Settings")]
        [MinValue(0)]
        [SerializeField] private int _goldBountyVariance = 5;

        [BoxGroup("Gold Bounty Settings")]
        [InfoBox("Calculated bounty range")]
        [ShowInInspector, ReadOnly]
        private string GoldBountyRange => $"{_baseGoldBounty - _goldBountyVariance} - {_baseGoldBounty + _goldBountyVariance}";

        public Sprite Icon => _icon;
        public string EntityName => _entityName;
        public string Description => _description;
        public int MaxHealth => _maxHealth;

        public int GetRandomGoldBounty()
        {
            int min = Mathf.Max(0, _baseGoldBounty - _goldBountyVariance);
            int max = _baseGoldBounty + _goldBountyVariance;
            return UnityEngine.Random.Range(min, max + 1);
        }
    }
}