using CastleFight.Core.Views;
using CastleFight.Main.Configs;
using UnityEngine;

namespace CastleFight.Core.Configs
{
    [CreateAssetMenu(menuName = "Core/Configs/Selector Handler Config")]
    public class SelectorHandlerConfig : ScriptableConfig
    {
        [SerializeField] private ClickableView _prefab;
        public ClickableView Prefab => _prefab;
    }
}
