using System;
using System.Collections;
using CastleFight.Core.BuildingsSystem;
using CastleFight.Core.BuildingsSystem.Factories;
using CastleFight.Core.Configs;
using CastleFight.Core.ConstructionSystem.Views;
using CastleFight.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Core.ConstructionSystem.Handlers
{
    public class ConstructHandler : MonoBehaviour, IConstructHandler
    {
        private Subject<IBuildingInstance> _onSelectBuilding = new();
        private Subject<Unit> _onEndBuild = new();
        private IBuildingInstance _currentBuilding;
        private IConstructView _view;
        [Inject] private IBuildingFactory _buildingFactory;
        [Inject] private ConstructHandlerConfig _config;

        public IObservable<IBuildingInstance> OnSelectBuilding => _onSelectBuilding;

        private static readonly Quaternion _defaultRotation = Quaternion.Euler(0, 90, 0);

        public IObservable<Unit> OnEndBuild => _onEndBuild;

        private void Awake()
        {
            _view = Instantiate(_config.Prefab);
        }

        public void SelectConstruct (IBuildingInstance building)
        {
            _currentBuilding = building;
            _onSelectBuilding.OnNext(building);
        }

        private void Update()
        {
            if (!EventSystem.current.IsBlockedByUI() && _currentBuilding != null)
            {
                if (Input.GetMouseButton(0) && _view.CanConstruct)
                {
                    Quaternion rotation = _defaultRotation;
                    Vector3 position = _view.Position;
                    BuildingInstance prefab = _currentBuilding as BuildingInstance;
                   var building =  _buildingFactory.Create(prefab, position, rotation);
                    building.TurnConstruct();
                    _onEndBuild.OnNext(Unit.Default);
                    _currentBuilding = null;
                }
            }
        }
    }
}