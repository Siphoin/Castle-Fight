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
            if (Input.GetKeyDown(KeyCode.E) && !EventSystem.current.IsBlockedByUI())
            {
                _factory.Create(_prefab, Vector3.zero, Quaternion.identity);
            }
        }

    }
}