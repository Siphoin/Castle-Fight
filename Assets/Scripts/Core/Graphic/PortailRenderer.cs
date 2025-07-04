using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CastleFight.Core.Graphic
{
    [RequireComponent(typeof(RawImage))]
    public class PortailRenderer : MonoBehaviour
    {
        [SerializeField, ReadOnly] private RawImage _rawImage;
        [SerializeField] private int _textureSize = 256;
        [SerializeField] private Vector3 _cameraOffset = new Vector3(0, 0, -2);
        [SerializeField] private Color _backgroundColor = new Color(0, 0, 0, 0);

#if UNITY_EDITOR
        [SerializeField] private Button _buttonScreenShot;
#endif

        private RenderTexture _renderTexture;
        private Camera _portraitCamera;
        private Portail _currentPortail;
        private Transform _previousParent;
        private Vector3 _previousPosition;
        private Quaternion _previousRotation;

        private void Awake()
        {
            CreateRenderTexture();
            SetupCamera();

#if UNITY_EDITOR
            _buttonScreenShot.onClick.AsObservable().Subscribe(_ =>
            {
                SaveRenderAsPNG();

            }).AddTo(this);
#endif
        }

        public void SetPortail(IPortaable portaable)
        {
            ReturnPreviousPortail();

            _currentPortail = portaable.Portail as Portail;
            if (_currentPortail == null) return;

            _previousParent = _currentPortail.transform.parent;
            _previousPosition = _currentPortail.transform.localPosition;
            _previousRotation = _currentPortail.transform.localRotation;

            _currentPortail.transform.SetParent(_portraitCamera.transform);
            _currentPortail.transform.localPosition = _currentPortail.PortraitPosition;
            _currentPortail.transform.localEulerAngles = _currentPortail.PortraitRotation;
        }

        [Button("Update View")]
        private void UpdateCameraAndPortrait()
        {
            if (_portraitCamera != null)
            {
                _portraitCamera.transform.localPosition = _cameraOffset;
            }

            if (_currentPortail != null)
            {
                _currentPortail.transform.localPosition = _currentPortail.PortraitPosition;
                _currentPortail.transform.localEulerAngles = _currentPortail.PortraitRotation;
            }
        }

#if UNITY_EDITOR
        [Button("Save Render as PNG")]
        private void SaveRenderAsPNG()
        {
            string folderPath = "Assets/RenderScreenshots";
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }

            Texture2D tex = new Texture2D(_renderTexture.width, _renderTexture.height, TextureFormat.RGBA32, false);
            RenderTexture.active = _renderTexture;
            tex.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            byte[] bytes = tex.EncodeToPNG();
            DestroyImmediate(tex);
            string timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            string filePath = $"{folderPath}/Render_{timestamp}.png";

            System.IO.File.WriteAllBytes(filePath, bytes);
            Debug.Log($"Render saved to: {filePath}");

            UnityEditor.AssetDatabase.Refresh();
        }
#endif

        private void ReturnPreviousPortail()
        {
            if (_currentPortail == null) return;

            _currentPortail.transform.SetParent(_previousParent);
            _currentPortail.transform.localPosition = _previousPosition;
            _currentPortail.transform.localRotation = _previousRotation;
        }

        private void SetupCamera()
        {
            var camObj = new GameObject("PortraitCamera");
            camObj.transform.SetParent(transform);
            camObj.transform.localPosition = _cameraOffset;

            _portraitCamera = camObj.AddComponent<Camera>();
            _portraitCamera.targetTexture = _renderTexture;
            _portraitCamera.clearFlags = CameraClearFlags.SolidColor;
            _portraitCamera.backgroundColor = _backgroundColor;
            _portraitCamera.orthographic = true;
            _portraitCamera.orthographicSize = 1;
            _portraitCamera.cullingMask = LayerMask.GetMask("Portail");
            _portraitCamera.nearClipPlane = -500f;
            _portraitCamera.depth = 0;
        }

        private void CreateRenderTexture()
        {
            _renderTexture = new RenderTexture(_textureSize, _textureSize, 16, RenderTextureFormat.ARGB32)
            {
                antiAliasing = 2
            };
            _rawImage.texture = _renderTexture;
        }

        private void OnValidate()
        {
            if (_rawImage == null)
            {
                _rawImage = GetComponent<RawImage>();
            }

#if UNITY_EDITOR
            if (!Application.isPlaying && _portraitCamera != null)
            {
                UnityEditor.EditorApplication.delayCall += () => UpdateCameraAndPortrait();
            }
#endif
        }

        private void OnDisable()
        {
            ReturnPreviousPortail();

            if (_renderTexture != null)
                _renderTexture.Release();
        }

        private void OnDestroy()
        {
            if (_portraitCamera != null)
                Destroy(_portraitCamera.gameObject);
        }
    }
}