using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace CastleFight.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public abstract class UIText : MonoBehaviour
    {
        [SerializeField, ReadOnly] private TextMeshProUGUI _component;

        protected TextMeshProUGUI Component => _component;

        protected virtual void OnValidate()
        {
            if (_component is null)
            {
                _component = GetComponent<TextMeshProUGUI>();
            }
        }
    }
}