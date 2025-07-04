﻿using System;
using CastleFight.Networking.Handlers;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CastleFight.Core.Components
{
    public class TimedDestroyHandler : MonoBehaviour
    {
        [SerializeField] private bool _destroyOnStart = false;

        [SerializeField] private float _timeDestroy = 5;
        [SerializeField, ReadOnly] private NetworkHandler _network;

        public float TimeDestroy { get => _timeDestroy; set => _timeDestroy = value; }

        private INetworkHandler Network
        {
            get
            {

                if (_network is null)
                {
                    _network = FindAnyObjectByType<NetworkHandler>();
                }
                return _network;
            }
        }

        public void DestroyObject ()
        {
            TurnDestroy().Forget();
        }

        private async UniTask TurnDestroy()
        {
            if (Network.IsConnected)
            {
                var token = this.destroyCancellationToken;
                await UniTask.Delay(TimeSpan.FromSeconds(_timeDestroy), cancellationToken: token);
                Network.DestroyNetworkObject(gameObject);
            }

            else
            {
                Destroy(gameObject, _timeDestroy);
            }
        }
    }
}