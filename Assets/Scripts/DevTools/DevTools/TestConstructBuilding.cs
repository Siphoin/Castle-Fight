using System.Collections;
using CastleFight.Core.BuildingsSystem;
using CastleFight.Core.BuildingsSystem.Factories;
using CastleFight.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CastleFight.DevTools
{
    public class TestConstructBuilding : MonoBehaviour
    {
        [SerializeField] private BuildingInstance _building;
        [Inject] private IBuildingFactory _buildingFactory;
        private readonly Quaternion _defaultRotation = Quaternion.Euler(0, 90, 0);

        private void Start()
        {
        }

        private void Update()
        {
            if (!EventSystem.current.IsBlockedByUI() && Input.GetKeyDown(KeyCode.B))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
                {
                   var building =  _buildingFactory.Create(_building, hit.point, _defaultRotation);
                    building.TurnConstruct();

                }
            }
        }
    }
}