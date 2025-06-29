using CastleFight.Main.Configs;
using UnityEngine;

namespace CastleFight.Core.Configs
{
    [CreateAssetMenu(menuName = "Core/Configs/Match Handler Config")]
    public class MatchConfig : ScriptableConfig
    {
        [SerializeField] private uint _startGold = 500;

        public uint StartGold => _startGold;
    }
}
