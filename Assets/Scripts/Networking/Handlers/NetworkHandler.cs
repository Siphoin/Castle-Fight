using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Zenject;
using CastleFight.Networking.Configs;
namespace CastleFight.Networking.Handlers
{
    [RequireComponent(typeof(UnityTransport))]
    [RequireComponent (typeof(NetworkManager))]
    public class NetworkHandler : MonoBehaviour, INetworkHandler
    {
        [Inject] private NetworkHandlerConfig _config;

        private void Awake()
        {
            var transport = GetComponent<UnityTransport>();
            if (transport != null)
            {
                transport.SetConnectionData(_config.DefaultIP, _config.Port);
            }
        }

        public void StartServer()
        {
            if (NetworkManager.Singleton.IsListening)
            {
                Debug.LogWarning("Already hosting or connected!");
                return;
            }

            NetworkManager.Singleton.StartHost();
            Debug.Log($"P2P Host started on {_config.DefaultIP}:{_config.Port}");
        }


        public void StartClient(string targetIP)
        {
            if (NetworkManager.Singleton.IsListening)
            {
                Debug.LogWarning("Already connected!");
                return;
            }

            string ip = string.IsNullOrEmpty(targetIP) ? _config.DefaultIP : targetIP;

            var transport = GetComponent<UnityTransport>();
            if (transport != null)
            {
                transport.SetConnectionData(ip, _config.Port);
            }

            NetworkManager.Singleton.StartClient();
            Debug.Log($"Client connecting to {ip}:{_config.Port}...");
        }

        public void Disconnect()
        {
            if (!NetworkManager.Singleton.IsListening)
            {
                Debug.LogError("Not connected!");
                return;
            }

            NetworkManager.Singleton.Shutdown();
            Debug.Log("Disconnected from the network.");
        }
    }
}
