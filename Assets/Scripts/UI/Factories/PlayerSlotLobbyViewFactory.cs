using System;
using CastleFight.Main;
using CastleFight.UI.Configs;
using CastleFight.UI.Views;
using UniRx;
using UnityEngine;
using Zenject;

namespace CastleFight.UI.Factories
{
    public class PlayerSlotLobbyViewFactory : IPlayerSlotLobbyViewFactory
    {
        [Inject] private DiContainer _container;
        [Inject] private PlayerSlotLobbyViewFactoryConfig _config;
        private PoolMono<PlayerSlotLobbyView> _pool;
        private Subject<IPlayerSlotLobbyView> _onSpawn = new();

        public IObservable<IPlayerSlotLobbyView> OnSpawn => _onSpawn;

        public IPlayerSlotLobbyView Create(PlayerSlotLobbyView prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (_pool is null)
            {
                _pool = new(prefab, null, _container, _config.StartCount, true);

            }

            var view = _pool.GetFreeElement();
            view.transform.SetParent(parent, false);
            _onSpawn.OnNext(view);
            return view;
        }
    }
}