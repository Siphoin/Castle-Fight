using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CastleFight.Core.UnitsSystem.Components
{
    [RequireComponent(typeof(Animator))]
    public class UnitAnimatorHandler : MonoBehaviour, IUnitAnimatorHandler
    {
        [SerializeField, ReadOnly] private Animator _animator;

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
        }
    }
}