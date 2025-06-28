using System.Collections;
using Sirenix.OdinInspector;
using Unity.Netcode.Components;
using UnityEngine;

namespace CastleFight.Core.UnitsSystem.Components
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NetworkAnimator))]
    public class UnitAnimatorHandler : MonoBehaviour, IUnitAnimatorHandler
    {
        [SerializeField, ReadOnly] private Animator _animator;
        [SerializeField, ReadOnly] private NetworkAnimator _networkAnimator;

        public void PlayAnimationAttack ()
        {
            PlayAnimation(UnitAnimationType.Attack);
        }

        private void PlayAnimation (UnitAnimationType animationType)
        {
            string name = animationType.ToString();
            _animator.Play(name);
        }

        private void OnValidate()
        {
            if (!_animator)
            {
                _animator = GetComponent<Animator>();
            }

            if (!_networkAnimator)
            {
                _networkAnimator = GetComponent<NetworkAnimator>();
            }

            _networkAnimator.Animator = _animator;
        }
    }
}