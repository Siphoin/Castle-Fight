using System.Collections;
using System.Linq;
using CastleFight.Core.Components;
using UnityEngine;

namespace CastleFight.Core.Handlers
{
    public class ComponentStateHandler : MonoBehaviour, IComponentStateHandler
    {
        private IDisableComponent[] _components;

        private void Awake()
        {
            var componentsOnSelf = GetComponents<IDisableComponent>();
            var componentsInParent = GetComponentsInParent<IDisableComponent>(true);
            var componentsInChildren = GetComponentsInChildren<IDisableComponent>(true);

            _components = componentsOnSelf
                .Union(componentsInParent)
                .Union(componentsInChildren)
                .Distinct()
                .ToArray();
        }

        public void Disable()
        {
            foreach (var component in _components)
            {
                component.Disable();
            }
        }
    }
}