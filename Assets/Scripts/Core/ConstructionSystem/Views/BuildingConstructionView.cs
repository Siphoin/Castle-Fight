using UnityEngine;
using UniRx;
using Unity.Netcode;
using CastleFight.Core.BuildingsSystem;
using System.Linq;
using CastleFight.Core.Graphic;
using Zenject;
using Sirenix.OdinInspector;

namespace CastleFight.Core.Views
{
    [RequireComponent(typeof(NetworkObject))]
    public class BuildingConstructionView : NetworkBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [ReadOnly, SerializeField] private BuildingInstance _buildingInstance;

        private Mesh[] _constructionStages;
        private CompositeDisposable _disposables = new();
        private NetworkVariable<int> _currentStageIndex = new();
        private IPortail _portail;

        public override void OnNetworkSpawn()
        {
            _portail = _buildingInstance.Portail;
            _constructionStages = _buildingInstance.Stats.ConstructMeshes?.ToArray();

            _buildingInstance.OnStartConstruct
                .TakeUntilDisable(this)
                .Subscribe(_ => InitializeConstruction())
                .AddTo(_disposables);

            _currentStageIndex.OnValueChanged += OnStageChanged;

            if (_buildingInstance.IsContructed)
            {
                enabled = false;
                return;
            }
        }

        private void InitializeConstruction()
        {
            if (_constructionStages?.Length > 0)
            {
                SetMesh(0);
                if (IsServer) _currentStageIndex.Value = 0;
            }

            _buildingInstance.HealthComponent.OnCurrentHealthChanged
                .TakeUntilDisable(this)
                .Subscribe(UpdateConstructionProgress)
                .AddTo(_disposables);
        }

        private void UpdateConstructionProgress(float health)
        {
            if (_constructionStages == null || _constructionStages.Length == 0)
                return;

            float maxHealth = _buildingInstance.HealthComponent.MaxHealth;
            float progress = Mathf.Clamp01(health / maxHealth);
            int newStage = Mathf.FloorToInt(progress * (_constructionStages.Length - 1));

            if (IsServer)
            {
                _currentStageIndex.Value = newStage;
            }
            else if (IsOwner)
            {
                UpdateStageServerRpc(newStage);
            }

            if (health >= maxHealth)
            {
                CompleteConstruction();
            }
        }

        private void OnStageChanged(int prevStage, int newStage)
        {
            if (_constructionStages != null &&
                newStage >= 0 &&
                newStage < _constructionStages.Length)
            {
                SetMesh(newStage);
            }
        }

        private void SetMesh(int stageIndex)
        {
            if (stageIndex < 0 ||
                stageIndex >= _constructionStages.Length ||
                _constructionStages[stageIndex] == null)
                return;

            _portail?.SetMesh(_constructionStages[stageIndex]);

            if (_meshFilter != null)
                _meshFilter.mesh = _constructionStages[stageIndex];
        }

        private void CompleteConstruction()
        {
            if (_constructionStages?.Length > 0)
                SetMesh(_constructionStages.Length - 1);

            Cleanup();
            enabled = false;
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateStageServerRpc(int stageIndex)
        {
            if (stageIndex >= 0 && stageIndex < _constructionStages.Length)
                _currentStageIndex.Value = stageIndex;
        }

        private void Cleanup()
        {
            _disposables?.Dispose();
            _currentStageIndex.OnValueChanged -= OnStageChanged;
        }

        public override void OnNetworkDespawn()
        {
            Cleanup();
        }

        private void OnValidate()
        {
            if (_buildingInstance == null)
                _buildingInstance = GetComponent<BuildingInstance>();
        }
    }
}