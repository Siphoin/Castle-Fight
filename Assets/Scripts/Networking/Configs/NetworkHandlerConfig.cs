using System.Collections.Generic;
using CastleFight.Main.Configs;
using CastleFight.Networking.Handlers;
using Unity.Netcode;
using UnityEngine;

namespace CastleFight.Networking.Configs
{
    [CreateAssetMenu(menuName = "Networking/Configs/Network Handler Config")]
    public class NetworkHandlerConfig : ScriptableConfig
    {
        [SerializeField] private string _defaultIP = "127.0.0.1";
        [SerializeField] private ushort _port = 7777;
        [SerializeField] private LogLevel _logLevel;
        [SerializeField] private NetworkObjectSpawnHandler _spawnHandlerPrefab;
        [SerializeField] private NetworkBehaviour[] _handlersPrefabs;
        public string DefaultIP => _defaultIP;
        public ushort Port => _port;

        public LogLevel LogLevel => _logLevel;

        internal IEnumerable<NetworkBehaviour> HandlersPrefabs => _handlersPrefabs;

        internal NetworkObjectSpawnHandler SpawnHandlerPrefab => _spawnHandlerPrefab;
    }

}
