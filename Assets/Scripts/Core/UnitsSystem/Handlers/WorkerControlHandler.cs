using System.Collections.Generic;
using System.Threading.Tasks;
using CastleFight.Core.BuildingsSystem;
using CastleFight.Core.ConstructionSystem.Events;
using CastleFight.Core.PhysicsSystem;
using CastleFight.Core.UnitsSystem.Components;
using CastleFight.Extensions;
using Core.ConstructionSystem.Handlers;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace CastleFight.Core.UnitsSystem.Handlers
{
    [RequireComponent(typeof(WorkerRepositoryRegistrer))]
    public class WorkerControlHandler : MonoBehaviour
    {
        [SerializeField, ReadOnly] private UnitInstance _unitInstance;
        [SerializeField, ReadOnly] private WorkerNavMesh _workerNavMesh;
        [SerializeField, ReadOnly] private ConstructHandler _constructHandler;

        private NavMeshPath _path;
        private bool _hasValidPath;
        private bool _isInBuildMode = false;
        private bool _isProcessingClick = false;
        private Queue<NewBuildingConstructEvent> _buildQueue = new Queue<NewBuildingConstructEvent>();
        private CompositeDisposable _disposables = new CompositeDisposable();

        public Queue<NewBuildingConstructEvent> BuildQueue => _buildQueue;

        private IConstructHandler ConstructHandler
        {
            get
            {
                if (!_constructHandler)
                {
                    _constructHandler = FindAnyObjectByType<ConstructHandler>();
                }
                return _constructHandler;
            }
        }

        private async void Start()
        {
            _path = new NavMeshPath();

            await UniTask.WaitUntil(() => ConstructHandler != null, cancellationToken: this.GetCancellationTokenOnDestroy());
            if (_unitInstance.IsOwner)
            {
                ConstructHandler.OnSelectBuilding.Subscribe(building =>
                {
                    if (_workerNavMesh.CurrentStateWorker != AI.WorkerUnitStateType.Constructing)
                    {
                        _isInBuildMode = true;
                        _workerNavMesh.SetWaitSelectPointBuild();
                    }
                }).AddTo(_disposables);

                ConstructHandler.OnEndBuild.Subscribe(ev =>
                {
                    if (_workerNavMesh.CurrentStateWorker != AI.WorkerUnitStateType.Constructing)
                    {
                        _isInBuildMode = false;
                        _workerNavMesh.MoveToBuild(ev);
                    }
                    else
                    {
                        _buildQueue.Enqueue(ev);
                    }
                }).AddTo(_disposables);
            }
        }

        private async void LateUpdate()
        {
            if (!_unitInstance.IsOwner)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0) && !_isProcessingClick)
            {
                if (!EventSystem.current.IsBlockedByUI() && _unitInstance.IsSelected)
                {
                    _isProcessingClick = true;

                    try
                    {
                        await UniTask.DelayFrame(1, PlayerLoopTiming.Update);

                        if (_workerNavMesh.CurrentStateWorker == AI.WorkerUnitStateType.WaitSelectPointBuild || _isInBuildMode)
                        {
                            return;
                        }

                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                        {
                            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
                            {
                                return;
                            }
                        }

                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
                        {
                            _hasValidPath = NavMesh.CalculatePath(transform.position, hit.point, NavMesh.AllAreas, _path);

                            if (_hasValidPath && _path.status == NavMeshPathStatus.PathComplete)
                            {
                                _buildQueue.Clear();
                                _workerNavMesh.SetPointMove(hit.point);
                            }
                        }
                    }
                    finally
                    {
                        _isProcessingClick = false;
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (!EventSystem.current.IsBlockedByUI() && _unitInstance.IsSelected)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        if (hit.collider.TryGetComponent(out HitBox hitBox))
                        {
                            if (hitBox.transform.parent != null)
                            {
                                if (hitBox.transform.parent.TryGetComponent(out IBuildingInstance building) && !building.IsContructed && building.IsMy)
                                {
                                    var point = NavMesh.SamplePosition(hit.point, out var hitNavMesh, 10, NavMesh.AllAreas);
                                    NewBuildingConstructEvent buildingConstructEvent = new NewBuildingConstructEvent(hitNavMesh.position, building);

                                    if (_workerNavMesh.CurrentStateWorker == AI.WorkerUnitStateType.Constructing)
                                    {
                                        _buildQueue.Enqueue(buildingConstructEvent);
                                    }
                                    else
                                    {
                                        _workerNavMesh.MoveToBuild(buildingConstructEvent);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }


        public bool HasTasksBuild()
        {
            return _buildQueue.Count > 0;
        }

        public void MoveToNextBuild()
        {
            if (_buildQueue.Count > 0)
            {
                var nextTask = _buildQueue.Dequeue();
                _workerNavMesh.MoveToBuild(nextTask);


            }

        }

        public void CancelAllBuildTasks()
        {
            _buildQueue.Clear();
        }
        private void OnValidate()
        {
            if (!_workerNavMesh) _workerNavMesh = GetComponent<WorkerNavMesh>();
            if (!_unitInstance) _unitInstance = GetComponent<UnitInstance>();
        }
    }
}