using UnityEngine;
using UnityEngine.EventSystems;

namespace CastleFight.Extensions
{
    public static class EventSystemExtensions
    {
        public static bool IsBlockedByUI(this EventSystem eventSystem)
        {
            bool isOverUI = eventSystem.IsPointerOverGameObject();

            bool isClicking = Input.GetMouseButtonDown(0) ||
                             (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

            return isOverUI || isClicking;
        }
    }
}
