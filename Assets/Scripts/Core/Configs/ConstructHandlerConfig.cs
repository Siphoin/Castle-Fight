using CastleFight.Core.ConstructionSystem.Views;
using CastleFight.Main.Configs;
using UnityEngine;

namespace CastleFight.Core.Configs
{
    [CreateAssetMenu(menuName = "Core/Configs/Construct Handler Config")]
    public class ConstructHandlerConfig : ScriptableConfig
    {
        [SerializeField] private ConstructView _prefab;

        public ConstructView Prefab => _prefab;
    }
}
