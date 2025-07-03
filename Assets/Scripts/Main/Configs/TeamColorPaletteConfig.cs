using CastleFight.Main.Configs;
using UnityEngine;
namespace CastleFight.Main.Configs
{
    [CreateAssetMenu(menuName = "Main/Configs/Color Team Palette Config")]
    public class TeamColorPaletteConfig : ScriptableConfig
    {
        [SerializeField] private Color[] _colors;

        public Color GetColor(int index)
        {
            return _colors[index];
        }
    }
}
