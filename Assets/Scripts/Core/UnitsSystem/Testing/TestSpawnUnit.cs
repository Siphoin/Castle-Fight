using CastleFight.Core.UnitsSystem.Factories;
using CastleFight.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CastleFight.Core.UnitsSystem.Testing
{
    public class TestSpawnUnit : MonoBehaviour
    {
        [SerializeField] private UnitInstance _prefab;
        [Inject] private IUnitFactory _factory;

        private void Awake()
        {
            if (!Application.isEditor)
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.U) && !EventSystem.current.IsBlockedByUI())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
                {
                    _factory.Create(_prefab, hit.point, Quaternion.identity);
                }
            }
        }
    }
}