using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace CastleFight.Core.UnitsSystem.Components
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class UnitNavMesh : MonoBehaviour, IUnitNavMesh
    {
        [SerializeField, ReadOnly] private NavMeshAgent _agent;

        private void OnValidate()
        {
            if (!_agent)
            {
                _agent = GetComponent<NavMeshAgent>();
            }
        }
    }
}