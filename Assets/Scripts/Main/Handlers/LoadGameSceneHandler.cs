using CastleFight.Main.Configs;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace CastleFight.Main.Handlers
{
    public class LoadGameSceneHandler : MonoBehaviour
    {
        [Inject] private LoadGameSceneHandlerConfig _config;

        private void Start()
        {
            SceneManager.LoadScene(_config.SceneName);
        }
    }
}