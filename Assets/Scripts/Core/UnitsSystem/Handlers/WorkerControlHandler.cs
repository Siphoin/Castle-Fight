using System.Collections;
using CastleFight.Core.UnitsSystem.Components;
using CastleFight.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace CastleFight.Core.UnitsSystem.Handlers
{
    public class WorkerControlHandler : MonoBehaviour
    {
        [SerializeField, ReadOnly] private UnitInstance _unitInstance;
        [SerializeField, ReadOnly] private WorkerNavMesh _workerNavMesh;


        private NavMeshPath _path;
        private bool _hasValidPath;

        private void Start()
        {
            enabled = _unitInstance.IsMy;
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
                        // Проверяем доступность точки через NavMesh
                        _hasValidPath = NavMesh.CalculatePath(transform.position, hit.point, NavMesh.AllAreas, _path);

                        if (_hasValidPath && _path.status == NavMeshPathStatus.PathComplete)
                        {
                            Vector3 direction = (hit.point - transform.position).normalized;
                            if (direction != Vector3.zero)
                            {
                                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                                _workerNavMesh.transform.rotation = lookRotation;
                            }

                            _workerNavMesh.SetPointMove(hit.point);
                        }
                        else
                        {
                            Debug.Log("Target point is not reachable");
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