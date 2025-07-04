using System;
using CastleFight.Core.Views.factories.Configs;
using CastleFight.Main;
using UniRx;
using UnityEngine;
using Zenject;

namespace CastleFight.Core.Views.factories
{
    public class ClickableViewFactory : IClickableViewFactory
    {
        [Inject] private DiContainer _container;
        [Inject] private ClickableViewFactoryConfig _config;

        private PoolMono<ClickableView> _pool;
        private Subject<IClickableView> _onSpawn = new();

        public IObservable<IClickableView> OnSpawn => _onSpawn;

        public IClickableView Create(ClickableView prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (_pool is null)
            {
                _pool = new(prefab, null, _container, _config.StartCount, true);
            }

            var view = _pool.GetFreeElement();
            _onSpawn.OnNext(view);
            return view;
        }
    }
}
