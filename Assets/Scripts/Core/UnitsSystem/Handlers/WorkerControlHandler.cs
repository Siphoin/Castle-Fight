using System.Collections;
using CastleFight.Core.BuildingsSystem;
using CastleFight.Core.ConstructionSystem.Events;
using CastleFight.Core.PhysicsSystem;
using CastleFight.Core.UnitsSystem.Components;
using CastleFight.Extensions;
using Core.ConstructionSystem.Handlers;
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

        private void Start()
        {



            enabled = _unitInstance.IsMy;

            if (!enabled)
            {
                return;
            }

            ConstructHandler.OnSelectBuilding.Subscribe(building =>
            {
                _workerNavMesh.SetWaitSelectPointBuild();

            }).AddTo(this);

            ConstructHandler.OnEndBuild.Subscribe(ev =>
            {
                _workerNavMesh.MoveToBuild(ev);

            }).AddTo(_workerNavMesh);
            _path = new NavMeshPath();
        }

        private void LateUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!EventSystem.current.IsBlockedByUI() && _unitInstance.IsSelected)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
                    {
                        _hasValidPath = NavMesh.CalculatePath(transform.position, hit.point, NavMesh.AllAreas, _path);

                        if (_hasValidPath && _path.status == NavMeshPathStatus.PathComplete)
                        {

                            _workerNavMesh.SetPointMove(hit.point);
                        }
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
                        Debug.Log(hit.collider.name);
                        if (hit.collider.TryGetComponent(out HitBox hitBox))
                        {
                            if (hitBox.transform.parent != null)
                            {
                                if (hitBox.transform.parent.TryGetComponent(out IBuildingInstance building) && !building.IsContructed)
                                {
                                    NewBuildingConstructEvent buildingConstructEvent = new NewBuildingConstructEvent(hit.point, building);
                                    _workerNavMesh.MoveToBuild(buildingConstructEvent);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnValidate()
        {
            if (!_workerNavMesh) _workerNavMesh = GetComponent<WorkerNavMesh>();
            if (!_unitInstance) _unitInstance = GetComponent<UnitInstance>();
        }

        


    }
}