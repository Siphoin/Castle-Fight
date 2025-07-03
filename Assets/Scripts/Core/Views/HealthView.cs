using System.Collections;
using CastleFight.Core.HealthSystem;
using CastleFight.Core.UI;
using TMPro;
using UniRx;
using UnityEngine;

namespace CastleFight.Core.Views
{
    public class HealthView : MonoBehaviour
    {
        [SerializeField] private FillSlider _fill;
        [SerializeField] private TextMeshProUGUI _textHP;
        private CompositeDisposable _disposable;
        private IHealthComponent _target;

        public void SetTarget(IHealthComponent healthComponent)
        {
            _disposable?.Clear();
            _disposable = new();
            _target = healthComponent;
            _fill.MaxValue = _target.MaxHealth;
            _target.OnCurrentHealthChanged.Subscribe(health =>
            {
                UpdateInfo();

                if (health <= 0)
                {
                    _disposable.Clear();
                }
            }).AddTo(_disposable);

            UpdateInfo();
        }

        private void UpdateInfo()
        {
            if (_textHP)
            {
                _textHP.text = FormatHealthText(_target.Health, _target.MaxHealth);
            }

            _fill.Value = _target.Health;
        }

        private string FormatHealthText(float currentHealth, float maxHealth)
        {
            string currentText = currentHealth >= 1f ?
                ((int)currentHealth).ToString() :
                currentHealth.ToString("0.##");

            return $"{currentText}/{maxHealth}";
        }

        private void OnDestroy()
        {
            _disposable?.Clear();
        }
    }
}