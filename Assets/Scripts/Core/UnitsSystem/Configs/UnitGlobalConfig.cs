using CastleFight.Main.Configs;
using UnityEngine;
namespace CastleFight.Core.UnitsSystem.Configs
{
    [CreateAssetMenu(menuName = "Core/Configs/Unit Global Config")]
    public class UnitGlobalConfig : ScriptableConfig
    {
        [SerializeField] private float _timeDestroyCorpse;
        [SerializeField] private float _rotationSpeed = 6;

        public float TimeDestroyCorpse => _timeDestroyCorpse;

        public float RotationSpeed => _rotationSpeed;
    }
}
