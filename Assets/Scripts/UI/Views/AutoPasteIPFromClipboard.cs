using CastleFight.Validation;
using TMPro;
using UnityEngine;
namespace CastleFight.UI.Views
{
    [RequireComponent(typeof(TMP_InputField))]
    public class AutoPasteIPFromClipboard : MonoBehaviour
    {
        private TMP_InputField _inputField;
        private void Awake()
        {
            string clipboardText = GUIUtility.systemCopyBuffer;
            if (!string.IsNullOrEmpty(clipboardText))
            {
                if (IPValidator.IsValidIPv4(clipboardText))
                {
                    clipboardText = clipboardText.Trim();
                    _inputField = GetComponent<TMP_InputField>();
                    _inputField.text = clipboardText;
                }
            }

        }
    }
}