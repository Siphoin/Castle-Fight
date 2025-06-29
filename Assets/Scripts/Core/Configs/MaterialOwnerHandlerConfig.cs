using CastleFight.Main.Configs;
using UnityEngine;

namespace CastleFight.Core.Configs
{
    [CreateAssetMenu(menuName = "Core/Configs/Material Owner Handler Config")]
    public class MaterialOwnerHandlerConfig : ScriptableConfig
    {
        [Header("Units")]
        [SerializeField] private Material[] _unitsMaterials;
        [Header("Buildings")]
        [SerializeField] private Material[] _buildingsMaterials;

        public Material GetUnitMaterial (int index)
        {
            return _unitsMaterials[index];
        }

        public Material GetBuildingMaterial (int index)
        {
            return _buildingsMaterials[index];
        }
    }
}
