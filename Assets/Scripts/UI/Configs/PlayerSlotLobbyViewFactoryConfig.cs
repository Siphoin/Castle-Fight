using CastleFight.Main.Configs;
using CastleFight.UI.Views;
using UnityEngine;

namespace CastleFight.UI.Configs
{
    [CreateAssetMenu(menuName = "UI/Configs/Player Slot Lobby View Factory Config")]
    public class PlayerSlotLobbyViewFactoryConfig : ScriptableConfig
    {
        [SerializeField] private PlayerSlotLobbyView _prefab;
        [SerializeField] private int _startCount = 12;

        public PlayerSlotLobbyView Prefab => _prefab;
        public int StartCount => _startCount;
    }
}
