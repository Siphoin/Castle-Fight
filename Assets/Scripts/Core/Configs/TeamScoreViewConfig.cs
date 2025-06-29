using CastleFight.Main.Configs;
using UnityEngine;
namespace CastleFight.Core.Configs
{
    [CreateAssetMenu(menuName = "Core/Configs/Team Score View Config")]
    public class TeamScoreViewConfig : ScriptableConfig
    {
        [SerializeField] private string _colorFirstTeam = "red";
        [SerializeField] private string _colorSecondTeam = "blue";

        public string ColorFirstTeam => _colorFirstTeam;
        public string ColorSecondTeam => _colorSecondTeam;
    }
}
