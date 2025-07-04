﻿using CastleFight.Core.HealthSystem;
using CastleFight.Core.BuildingsSystem.SO;
using CastleFight.Core.BuildingsSystem.Components;
using Sirenix.OdinInspector;
using UnityEngine;
using CastleFight.Core.Handlers;
using CastleFight.Core.Graphic;
using CastleFight.Core.ConstructionSystem;
using CastleFight.Core.Views;
using Unity.Netcode;
using System;
using UniRx;
using CastleFight.Core.ConstructionSystem.Views;

namespace CastleFight.Core.BuildingsSystem
{
    [RequireComponent(typeof(BuildingSpawnHandler))]
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(BuildingObjectRepository))]
    [RequireComponent(typeof(ComponentDisableAfterDeadHandler))]
    [RequireComponent(typeof(SelectorHandler))]
    [RequireComponent(typeof(BuildingConstructHandler))]
    [RequireComponent(typeof(BuildingConstructionView))]
    public class BuildingInstance : OwnedEntity, IBuildingInstance
    {
        [SerializeField, ReadOnly] private BuildingSpawnHandler _spawnHandler;
        [SerializeField, ReadOnly] private HealthComponent _healthComponent;
        [SerializeField] private ScriptableBuuidingEntity _stats;
        [SerializeField] private Portail _portail;
        [SerializeField, ReadOnly] private SelectorHandler _selectorHandler;
        [SerializeField, ReadOnly] private BuildingConstructHandler _constructHandler;
        [SerializeField, ReadOnly] private BuildingModelView _buildingView;
        private NetworkVariable<bool> _isConstructed = new NetworkVariable<bool>(true);
        private Subject<bool> _onStartConstruct = new();

        public bool IsContructed
        {
            get => _isConstructed.Value;
            internal set => _isConstructed.Value = value;
        }
        public bool HasConstruction => _stats.BuildTime > 0;

        public IHealthComponent HealthComponent => _healthComponent;
        public ScriptableBuuidingEntity Stats => _stats;

        public string Name => _stats.EntityName;
        public string DamageInfo => _stats.GetDamageInfo();
        public IPortail Portail => _portail;
        public float SelectionScale => _stats.SelectionScale;
        public IBuildingConstructHandler ConstructHandler => _constructHandler;

        public IObservable<bool> OnStartConstruct => _onStartConstruct;

        public bool IsSelected => _selectorHandler.IsSelect;

        public IBuildingModelView BuildingView => _buildingView;

        public Vector3 SpawnPoint => _spawnHandler.SpawnPoint;

        public bool Isinvulnerable => _stats.IsInvulnerable;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsMy)
            {
                _healthComponent.SetHealthData(_stats.MaxHealth);
            }

            _isConstructed.OnValueChanged += ConstructFlagChanged;
        }

        private void ConstructFlagChanged(bool previousValue, bool newValue)
        {
            if (!newValue)
            {
                _onStartConstruct.OnNext(true);
            }
        }

        public void SetStateSelect(bool visible)
        {
            _selectorHandler.SetVisible(visible);
        }

        public void TurnConstruct()
        {
            if (HasConstruction && IsContructed)
            {
                IsContructed = false;
                _healthComponent.TurnConstructHealth();
                _constructHandler.TurnConstruct();
                _onStartConstruct.OnNext(true);
            }
        }

        private void OnValidate()
        {
            if (!_healthComponent) _healthComponent = GetComponent<HealthComponent>();
            if (!_selectorHandler) _selectorHandler = GetComponent<SelectorHandler>();
            if (!_constructHandler) _constructHandler = GetComponent<BuildingConstructHandler>();
            if (!_buildingView) _buildingView = GetComponentInChildren<BuildingModelView>();
            if (!_spawnHandler) _spawnHandler = GetComponent<BuildingSpawnHandler>();
        }
    }
}