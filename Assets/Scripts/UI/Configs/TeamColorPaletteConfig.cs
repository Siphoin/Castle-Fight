using CastleFight.Main.Configs;
using UnityEngine;
namespace CastleFight.UI.Configs
{
    [CreateAssetMenu(menuName = "UI/Configs/Color Team Palette Config")]
    public class TeamColorPaletteConfig : ScriptableConfig
    {
        [SerializeField] private Color[] _colors;

        public Color GetColor(int index)
        {
            return _colors[index];
        }

        public Color GetColor(PlayerColorType playerColorType)
        {
            int index = (int)playerColorType;
            return GetColor(index);
        }
    }
}
