using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CastleFight.Extensions
{
    public static class EventSystemExtensions
    {
        public static bool IsBlockedByUI(this EventSystem eventSystem)
        {
            Vector2 position = Vector2.zero;

#if UNITY_STANDALONE
            position = new(Input.mousePosition.x, Input.mousePosition.y);
#endif
#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0)
            {
                var positionTouch = Input.GetTouch(0).position;
                position = new(positionTouch.x, positionTouch.y);
            }
#endif

            var eventDataCurrentPosition = new PointerEventData(eventSystem)
            {
                position = new(position.x, position.y)
            };

            List<RaycastResult> results = new();
            eventSystem.RaycastAll(eventDataCurrentPosition, results);

            return results.Any(result => result.gameObject.layer == LayerMask.NameToLayer("UI"));
        }
    }
}
