using UnityEngine;
using CastleFight.Main.Configs;

namespace CastleFight.Core.Configs
{
    [CreateAssetMenu(menuName = "Core/Configs/Construction Config")]
    public class BuildingConstructionConfig : ScriptableConfig
    {
        [SerializeField, Range(0f, 1f)]
        private float _stage1Threshold = 0.33f;

        [SerializeField, Range(0f, 1f)]
        private float _stage2Threshold = 0.66f;

        public int GetStageIndex(float progress, int maxStages)
        {
            if (maxStages == 3)
            {
                if (progress < _stage1Threshold) return 0;
                if (progress < _stage2Threshold) return 1;
                return 2;
            }

            return Mathf.FloorToInt(progress * maxStages);
        }
    }
}