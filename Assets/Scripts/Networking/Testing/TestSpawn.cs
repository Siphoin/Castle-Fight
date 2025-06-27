using System;
using CastleFight.Networking.Handlers;
using UnityEngine;
using Zenject;

namespace CastleFight.Networking.Testing
{
    public class TestSpawn : MonoBehaviour
    {
        [Inject] private INetworkHandler _network;

        [SerializeField] private GameObject _testPrefab;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _network.SpawnNetworkObject(_testPrefab, OnSpawn, ownerClientId: _network.LocalClientId);
            }
        }

        private void OnSpawn(GameObject result)
        {
            Debug.Log(result.name);
        }
    }
}