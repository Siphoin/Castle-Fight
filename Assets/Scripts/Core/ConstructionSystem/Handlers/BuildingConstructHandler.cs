using System;
using System.Threading;
using CastleFight.Core.HealthSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using CastleFight.Core.BuildingsSystem;

namespace CastleFight.Core.ConstructionSystem
{
    [RequireComponent(typeof(HealthComponent))]
    public class BuildingConstructHandler : OwnedEntity, IBuildingConstructHandler
    {
        [SerializeField, ReadOnly] private HealthComponent _healthComponent;
        [SerializeField, ReadOnly] private BuildingInstance _buildingInstance;

        private float _regenPerSecond;

        public IHealthComponent HealthComponent => _healthComponent;
        public bool IsConstructing { get; private set; }

        public void TurnConstruct()
        {
            if (IsConstructing) return;

            IsConstructing = true;

            float totalHPToRegen = _buildingInstance.Stats.MaxHealth - 1;
            float buildTime = _buildingInstance.Stats.BuildTime;
            _regenPerSecond = totalHPToRegen / buildTime;

            ConstructionProcess().Forget();
        }

        private async UniTaskVoid ConstructionProcess()
        {
            try
            {
                var ct = this.GetCancellationTokenOnDestroy();
                float buildTime = _buildingInstance.Stats.BuildTime;
                float totalHPToRegen = _buildingInstance.Stats.MaxHealth - 1;
                float startHealth = 1f;

                // Установка начального здоровья
                _healthComponent.SetHealth(startHealth);

                float elapsed = 0f;
                const float updateInterval = 0.05f;

                while (elapsed < buildTime)
                {
                    ct.ThrowIfCancellationRequested();

                    float progress = Mathf.Clamp01(elapsed / buildTime);
                    float targetHealth = startHealth + totalHPToRegen * progress;

                    // Устанавливаем точное значение здоровья
                    _healthComponent.SetHealth(targetHealth);

                    await UniTask.Delay(TimeSpan.FromSeconds(updateInterval), cancellationToken: ct);
                    elapsed += updateInterval;
                }

                // Финализируем
                _healthComponent.SetHealth(_buildingInstance.Stats.MaxHealth);
                ConstructionComplete();
            }
            catch (OperationCanceledException)
            {
                // Строительство отменено
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка при строительстве: {e}");
            }
        }




        private void ConstructionComplete()
        {
            IsConstructing = false;
            _buildingInstance.IsContructed = true;
        }


        private void OnValidate()
        {
            if (_healthComponent is null)
                _healthComponent = GetComponent<HealthComponent>();

            if (_buildingInstance is null)
                _buildingInstance = GetComponent<BuildingInstance>();
        }
    }
}