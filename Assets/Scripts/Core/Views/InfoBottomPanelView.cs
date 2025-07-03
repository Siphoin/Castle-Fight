using System.Collections;
using CastleFight.Core;
using CastleFight.Core.BuildingsSystem;
using CastleFight.Core.Graphic;
using CastleFight.Core.Views;
using TMPro;
using UniRx;
using UnityEngine;

namespace Assets.Scripts.Core.Views
{
    public class InfoBottomPanelView : MonoBehaviour, IInfoBottomPanelView
    {
        private IClickableObject _target;
        [SerializeField] private HealthView _healthView;
        [SerializeField] private TextMeshProUGUI _damageInfo;
        [SerializeField] private TextMeshProUGUI _nameInfo;
        [SerializeField] private PortailRenderer _portailRenderer;
        [SerializeField] private RectTransform _infoIncome;
        [SerializeField] private TextMeshProUGUI _textIncome;
        private CompositeDisposable _disposable;

        private void Start()
        {
            SetStateVisible(false);
        }

        public void SetTarget (IClickableObject target)
        {
            _target = target;
            _damageInfo.text = _target.DamageInfo;
            _healthView.SetTarget(target.HealthComponent);
            _nameInfo.text = target.Name;
            _disposable?.Clear();
            _disposable = new();
            _portailRenderer.SetPortail(target);
            _target.HealthComponent.OnCurrentHealthChanged.Subscribe(health =>
            {
                if (health <= 0)
                {
                    SetStateVisible(false);
                }

            }).AddTo(_disposable);

            _infoIncome.gameObject.SetActive(false);

            if (target is IBuildingInstance building)
            {
                _infoIncome.gameObject.SetActive(true);
                _textIncome.text = $"<color=yellow>Income:</color> {building.Stats.Income}";
            }

            SetStateVisible(true);
        }

        private void SetStateVisible (bool visible)
        {
            gameObject.SetActive(visible);
        }

        private void OnDisable()
        {
            _disposable?.Clear();
        }
    }
}