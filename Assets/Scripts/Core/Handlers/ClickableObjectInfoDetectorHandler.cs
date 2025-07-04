using System.Collections;
using Assets.Scripts.Core.Views;
using CastleFight.Core.PhysicsSystem;
using CastleFight.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CastleFight.Core.Handlers
{
    public class ClickableObjectInfoDetectorHandler : MonoBehaviour
    {
        private EventSystem _eventSystem;
        private InfoBottomPanelView _infoBottomPanelView;
        [SerializeField] private LayerMask _layerMask;
        private IInfoBottomPanelView View
        {
            get
            {
                if (_infoBottomPanelView is null)
                {
                    _infoBottomPanelView = FindAnyObjectByType<InfoBottomPanelView>(FindObjectsInactive.Include);
                }
                return _infoBottomPanelView;
            }
        }
        private void Awake()
        {
            _eventSystem = EventSystem.current;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !_eventSystem.IsBlockedByUI())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, _layerMask))
                {
                    if (hit.collider.TryGetComponent(out IClickableObject clickableObject) && !clickableObject.HealthComponent.IsDead)
                    {
                        View.SetTarget(clickableObject);
                        clickableObject.SetStateSelect(true);

                    }

                    if (hit.collider.TryGetComponent(out HitBox hitBox))
                    {
                        if (hitBox.transform.parent != null)
                        {
                            if (hitBox.transform.parent.TryGetComponent(out IClickableObject hitObject))
                            {
                                View.SetTarget(hitObject);
                                hitObject.SetStateSelect(true);
                            }
                        }
                    }
                }
            }
        }
    }
}