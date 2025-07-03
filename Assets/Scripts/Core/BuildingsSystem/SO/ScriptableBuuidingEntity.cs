using System.Collections.Generic;
using CastleFight.Core.SO;
using CastleFight.Core.UnitsSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CastleFight.Core.BuildingsSystem.SO
{
    [CreateAssetMenu(menuName = "Core/Buildings/New Building")]
    public class ScriptableBuuidingEntity : ScriptableLiveEntity
    {
        [Title("Training Settings")]
        [BoxGroup("Training Settings")]
        [MinValue(0.1f)]
        [SerializeField] private float _trainSpeed = 1f;

        [BoxGroup("Training Settings")]
        [AssetSelector(Paths = "Assets/Prefabs/Units")]
        [SerializeField] private UnitInstance _trainableUnit;

        [BoxGroup("Building Stats")]
        [SerializeField] private int _buildTime = 30;

        [BoxGroup("Building Stats")]
        [SerializeField] private uint _income = 5;

        [BoxGroup("Construct Data")]
        [SerializeField] private Mesh[] _constructMeshes;

        public float TrainSpeed => _trainSpeed;
        public UnitInstance TrainableUnit => _trainableUnit;
        public int BuildTime => _buildTime;

        public uint Income => _income;

        public IEnumerable<Mesh> ConstructMeshes => _constructMeshes;
    }

}