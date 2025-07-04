using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CastleFight.Core.Views
{
    public class ClickableView : MonoBehaviour, IClickableView
    {
        [SerializeField] private MeshRenderer _selection;
        private IClickableObject _target;
        private CompositeDisposable _disposable;
        private Quaternion _initialRotation;

        public bool IsActive => _selection.gameObject.activeSelf;

        private void Awake()
        {
            _initialRotation = transform.rotation;
        }

        public void SetVisibleSelect(bool visible)
        {
            _selection.gameObject.SetActive(visible);

            if (!visible)
            {
                transform.SetParent(null, false);
                transform.position = Vector3.zero;
            }
        }

        private void LateUpdate()
        {
            transform.rotation = _initialRotation;
        }

        public void SetTarget(IClickableObject target)
        {
            _disposable = new();
            _target = target;
            _target.HealthComponent.OnCurrentHealthChanged.Subscribe(health =>
            {
                if (health <= 0)
                {
                    transform.SetParent(null, false);
                    transform.position = Vector3.zero;
                    transform.rotation = _initialRotation;
                    _disposable.Clear();
                    _selection.gameObject.SetActive(false);
                }
            }).AddTo(_disposable);

            Vector3 scale = new(target.SelectionScale, target.SelectionScale, target.SelectionScale);
            transform.localScale = scale;
        }

        private void OnDisable()
        {
            _disposable?.Clear();
        }
    }
}