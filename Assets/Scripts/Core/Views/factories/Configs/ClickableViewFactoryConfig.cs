using CastleFight.Main.Configs;
using UnityEngine;

namespace CastleFight.Core.Views.factories.Configs
{
    [CreateAssetMenu(menuName = "Core/Configs/Clickable View Factory Config")]
    public class ClickableViewFactoryConfig : ScriptableConfig
    {
        [SerializeField] private int _startCount = 15;
        [SerializeField] private ClickableView _prefab;

        public int StartCount => _startCount;
    }
}
