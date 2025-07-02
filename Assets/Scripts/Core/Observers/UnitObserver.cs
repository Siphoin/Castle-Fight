using System.Collections;
using CastleFight.Core.HealthSystem;
using CastleFight.Core.UnitsSystem;
using CastleFight.Core.UnitsSystem.Factories;
using CastleFight.Networking.Handlers;
using UniRx;
using UnityEngine;
using Zenject;

namespace CastleFight.Core.Observers
{
    public class UnitObserver : MonoBehaviour
    {
        [Inject] private IUnitFactory _unitFactory;
        [Inject] private INetworkHandler _networkHandler;

        private void Start()
        {
            _unitFactory.OnSpawn.Subscribe(unit =>
            {
                ListenUnit(unit);

            }).AddTo(this);
        }

        private void ListenUnit(IUnitInstance unit)
        {
            if (unit.IsMy)
            {
                CompositeDisposable disposables = new CompositeDisposable();
                Debug.Log(nameof(ListenUnit));
                unit.Combat.OnKill.Subscribe(killingObject =>
                {

                    HealthComponent healthComponent = killingObject as HealthComponent;

                    if (healthComponent.TryGetComponent(out IUnitInstance unitComponent))
                    {
                        var player = _networkHandler.Players.GetPlayerById(unit.OwnerId);
                        var gains = player.Gold + unitComponent.Stats.GetRandomGoldBounty();
                        if (gains != player.Gold)
                        {
                            _networkHandler.Players.SetPlayerGold(player.ClientId, (uint)gains);
                        }
                    }

                }).AddTo(disposables);

                unit.HealthComponent.OnCurrentHealthChanged.Subscribe(health =>
                {
                    if (health <= 0)
                    {
                        disposables.Clear();
                        disposables.Dispose();
                    }

                }).AddTo(disposables);
            }
        }
    }
}