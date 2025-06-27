using UnityEngine;
namespace CastleFight.Main.Configs
{
    [CreateAssetMenu(menuName = "System/Configs/Startup Scene/Load Game Scene Handler Config")]
    public class LoadGameSceneHandlerConfig : ScriptableConfig
    {
        [SerializeField] private string _sceneName = "Game";

        public string SceneName => _sceneName;
    }
}
