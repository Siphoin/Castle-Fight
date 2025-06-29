using CastleFight.Validation;
using Sirenix.Utilities.Editor;
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
            if (Clipboard.TryPaste(out string data))
            {
                if (IPValidator.IsValidIPv4(data))
                {
                    data = data.Trim();
                    _inputField = GetComponent<TMP_InputField>();
                    _inputField.text = data;
                }
            }
        }
    }
}