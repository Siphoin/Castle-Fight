using CastleFight.Main.Configs;
using UnityEngine;

namespace CastleFight.Core.Configs
{
    [CreateAssetMenu(menuName = "Core/Configs/Lobby Handler Config")]
    public class LobbyHandlerConfig : ScriptableConfig
    {
        [SerializeField] private ushort _timeToStart = 5;

        public ushort TimeToStart => _timeToStart;
    }
}
