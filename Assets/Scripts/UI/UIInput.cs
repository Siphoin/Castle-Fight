using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;

namespace CastleFight.UI
{
    [RequireComponent(typeof(TMP_InputField))]
    public class UIInput : MonoBehaviour
    {
        [SerializeField, ReadOnly] private TMP_InputField _inputField;
        [SerializeField] private bool _trimming = true;
        [SerializeField] private bool _clearOnDisable = true;

        protected TMP_InputField Component => _inputField;
        public string Text => _inputField.text;

        protected virtual void Awake ()
        {
            if (_trimming)
            {
                _inputField.onEndEdit.AsObservable().Subscribe(_ =>
                {
                    _inputField.text = _inputField.text.Trim();

                }).AddTo(this);
            }
        }

        protected virtual void OnEnable()
        {
            if (_clearOnDisable)
            {
                _inputField.text = string.Empty;
            }
        }

        protected virtual void OnValidate ()
        {
            if (_inputField is null)
            {
                _inputField = GetComponent<TMP_InputField>();
            }
        }
    }
}