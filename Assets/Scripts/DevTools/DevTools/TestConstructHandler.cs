using System.Collections;
using CastleFight.Core.BuildingsSystem;
using Core.ConstructionSystem.Handlers;
using UnityEngine;

namespace CastleFight.DevTools
{
    public class TestConstructHandler : MonoBehaviour
    {
        [SerializeField] private BuildingInstance _building;

        private IConstructHandler Handler => FindAnyObjectByType<ConstructHandler>();

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.C))
            {
                Handler.SelectConstruct(_building);
            }
        }
    }
}