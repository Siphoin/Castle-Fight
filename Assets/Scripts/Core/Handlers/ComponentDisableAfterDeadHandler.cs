using System.Collections;
using CastleFight.Core.HealthSystem;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace CastleFight.Core.Handlers
{
    [RequireComponent(typeof(ComponentStateHandler))]
    public class ComponentDisableAfterDeadHandler : MonoBehaviour
    {
        private IComponentStateHandler _handler;
        [SerializeField, ReadOnly] private HealthComponent _healthComponent;

        private void Awake()
        {
            _handler = GetComponent<ComponentStateHandler>();

            _healthComponent.OnCurrentHealthChanged.Subscribe(health =>
            {
                if (health <= 0)
                {
                    _handler.Disable();
                }

            }).AddTo(this);
        }

        private void OnValidate()
        {
            if (_healthComponent is null)
            {
                _healthComponent = GetComponent<HealthComponent>();
            } 
        }
    }
}