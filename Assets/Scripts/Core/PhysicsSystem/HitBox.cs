using CastleFight.Core.Components;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CastleFight.Core.PhysicsSystem
{
    public class HitBox : MonoBehaviour, IDisableComponent
    {
        [SerializeField, ReadOnly] private Collider _collider;
        [SerializeField, ReadOnly] private Rigidbody _rigidbody;

        public void Disable()
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            _collider.enabled = false;
        }

        private void OnValidate()
        {
            if (_rigidbody is null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }

            if (_collider is null)
            {
                _collider = GetComponent<Collider>();
            }
        }
    }
}