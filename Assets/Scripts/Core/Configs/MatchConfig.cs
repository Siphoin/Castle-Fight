using CastleFight.Core.BuildingsSystem;
using CastleFight.Core.UnitsSystem;
using CastleFight.Main.Configs;
using UnityEngine;

namespace CastleFight.Core.Configs
{
    [CreateAssetMenu(menuName = "Core/Configs/Match Handler Config")]
    public class MatchConfig : ScriptableConfig
    {
        [SerializeField] private uint _startGold = 500;
        [SerializeField] private BuildingInstance _castlePrefab;
        [SerializeField] private UnitInstance _workerPrefab;

        public uint StartGold => _startGold;

        public BuildingInstance CastlePrefab => _castlePrefab;

        public UnitInstance WorkerPrefab => _workerPrefab;
    }
}
