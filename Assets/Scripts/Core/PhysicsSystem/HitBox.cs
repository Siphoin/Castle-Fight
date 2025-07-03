using CastleFight.Core.Components;
using CastleFight.Networking.Handlers;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace CastleFight.Core.PhysicsSystem
{
    public class HitBox : MonoBehaviour, IDisableComponent
    {
        private const string ALLY_TAG_BOX = "AllyHitBox";

        [SerializeField, ReadOnly] private Collider _collider;
        [SerializeField, ReadOnly] private Rigidbody _rigidbody;
        [Inject] private INetworkHandler _networkHandler;
        private ITeamableObject _owner;
        private string _defaultTag;

        private void OnEnable()
        {
            _defaultTag = tag;
            if (transform.parent != null)
            {
                if (transform.parent.TryGetComponent(out _owner))
                {
                    _owner.OnPlayerOwnerChanged.Subscribe(player =>
                    {

                        if (_owner.IsAlly(_networkHandler.Players.LocalPlayer))
                        {
                            tag = ALLY_TAG_BOX;
                        }

                        else
                        {
                            tag = _defaultTag;
                        }

                    }).AddTo(this);
                }
            }

        }

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