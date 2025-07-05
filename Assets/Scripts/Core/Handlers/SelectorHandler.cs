using CastleFight.Core.Configs;
using CastleFight.Core.Views;
using CastleFight.Core.Views.factories;
using UnityEngine;
using Zenject;

namespace CastleFight.Core.Handlers
{
    public class SelectorHandler : MonoBehaviour, ISelectorHandler
    {
        [Inject] private IClickableViewFactory _factory;
        [Inject] private SelectorHandlerConfig _config;
        private IClickableObject _clickableObject;
        public bool IsSelect => Current == this;
        private ClickableView ActiveView { get; set; }
        private static SelectorHandler Current { get; set; }

        private void Awake()
        {
            TryGetComponent(out _clickableObject);
        }
        public void SetVisible(bool visible)
        {
            if (Current == this)
            {
                return;
            }
            try
            {
                Current?.ActiveView?.gameObject.SetActive(false);
            }
            catch
            {
            }
           

            if (visible)
            {
                ActiveView = _factory.Create(_config.Prefab, Vector3.zero, Quaternion.identity) as ClickableView;
                ActiveView.transform.SetParent(transform, false);
                ActiveView.transform.localPosition = Vector3.zero;
                ActiveView.transform.localEulerAngles = Vector3.zero;
                if (_clickableObject != null)
                {
                    ActiveView.SetTarget(_clickableObject);
                }
                ActiveView.SetVisibleSelect(true);

                Current = this;
            }
        }
    }
}