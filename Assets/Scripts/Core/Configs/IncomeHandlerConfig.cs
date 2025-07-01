using System;
using CastleFight.Main.Configs;
using UnityEngine;

namespace CastleFight.Core.Configs
{
    [CreateAssetMenu(menuName = "Core/Configs/Income Handler Config")]
    public class IncomeHandlerConfig : ScriptableConfig
    {
        [SerializeField] private float _timeIncome = 3;

        public TimeSpan TimeIncome => TimeSpan.FromSeconds(_timeIncome);
    }
}
