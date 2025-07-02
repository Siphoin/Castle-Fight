using CastleFight.Main.Configs;
using DG.Tweening;
using UnityEngine;

namespace CastleFight.UI.Configs
{
    [CreateAssetMenu(menuName = "UI/Configs/Gold View Config")]
    public class GoldViewConfig : ScriptableConfig
    {
        [SerializeField] private float _speedAnimation = 0.5f;
        [SerializeField] private Ease _ease = Ease.Linear;
        [SerializeField] private Color _colorIncrement = Color.yellow;
        [SerializeField] private Color _colorDecrement = Color.red; 

        public float SpeedAnimation => _speedAnimation;
        public Ease Ease => _ease;

        public Color ColorIncrement => _colorIncrement;
        public Color ColorDecrement => _colorDecrement;
    }
}
