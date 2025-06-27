using CastleFight.Main.Configs;
using UnityEngine;

namespace CastleFight.Networking.Configs
{
    [CreateAssetMenu(menuName = "Networking/Configs/Network Handler Config")]
    public class NetworkHandlerConfig : ScriptableConfig
    {
        [SerializeField] private string _defaultIP = "127.0.0.1";
        [SerializeField] private ushort _port = 7777;

        public string DefaultIP => _defaultIP;
        public ushort Port => _port;
    }
}
