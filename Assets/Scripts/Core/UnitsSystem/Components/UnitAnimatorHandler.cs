using System.Collections;
using Sirenix.OdinInspector;
using UniRx;
using Unity.Netcode.Components;
using UnityEngine;

namespace CastleFight.Core.UnitsSystem.Components
{
    [RequireComponent(typeof(Animator))]
    public class UnitAnimatorHandler : MonoBehaviour, IUnitAnimatorHandler
    {
        [SerializeField, ReadOnly] private Animator _animator;
        [SerializeField] private UnitInstance _unitInstance;

        private void Start()
        {
            if (_unitInstance.IsMy)
            {
                _unitInstance.HealthComponent.OnCurrentHealthChanged.Subscribe(health =>
                {
                    if (health <= 0)
                    {
                        PlayAnimation(UnitAnimationType.Death);
                    }

                }).AddTo(this);
            }
        }

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