using CastleFight.Main.Configs;
using UnityEngine;

namespace CastleFight.Core.HealthSystem.Configs
{
    [CreateAssetMenu(menuName = "Core/Configs/Health Component Config")]
    public class HealthComponentConfig : ScriptableConfig
    {
        [SerializeField] private float _startHealthConstructBuilding = 1;

        public float StartHealthConstructBuilding => _startHealthConstructBuilding;
    }
}
