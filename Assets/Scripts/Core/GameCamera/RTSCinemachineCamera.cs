using UnityEngine;
using Unity.Cinemachine;
using CastleFight.Core.GameCamera;
namespace CastleFight.Core.GameCamer
{
    [RequireComponent(typeof(CinemachineCamera))]
    [AddComponentMenu("Camera/RTS Cinemachine Camera")]
    public class RTSCinemachineCamera : MonoBehaviour, IGameCamera
    {
        #region Serialized Fields

        [Header("Movement Settings")]
        [SerializeField] private float keyboardMovementSpeed = 5f;
        [SerializeField] private float screenEdgeMovementSpeed = 3f;
        [SerializeField] private float followingSpeed = 5f;
        [SerializeField] private float panningSpeed = 10f;

        [Header("Rotation Settings")]
        [SerializeField] private float keyboardRotationSpeed = 3f;
        [SerializeField] private float mouseRotationSpeed = 10f;

        [Header("Height/Zoom Settings")]
        [SerializeField] private bool autoHeight = true;
        [SerializeField] private LayerMask groundMask = -1;
        [SerializeField] private float minHeight = 5f;
        [SerializeField] private float maxHeight = 15f;
        [SerializeField] private float heightDampening = 5f;
        [SerializeField] private float keyboardZoomSensitivity = 2f;
        [SerializeField] private float scrollWheelZoomSensitivity = 25f;

        [Header("Map Limits")]
        [SerializeField] private bool limitMap = true;
        [SerializeField] private float limitX = 50f;
        [SerializeField] private float limitY = 50f;

        [Header("Input Settings")]
        [SerializeField] private bool useScreenEdgeInput = true;
        [SerializeField] private float screenEdgeBorder = 25f;
        [SerializeField] private bool useKeyboardInput = true;
        [SerializeField] private string horizontalAxis = "Horizontal";
        [SerializeField] private string verticalAxis = "Vertical";
        [SerializeField] private bool usePanning = true;
        [SerializeField] private KeyCode panningKey = KeyCode.Mouse2;
        [SerializeField] private bool useKeyboardZooming = true;
        [SerializeField] private KeyCode zoomInKey = KeyCode.E;
        [SerializeField] private KeyCode zoomOutKey = KeyCode.Q;
        [SerializeField] private bool useScrollWheelZooming = true;
        [SerializeField] private string zoomingAxis = "Mouse ScrollWheel";
        [SerializeField] private bool useKeyboardRotation = true;
        [SerializeField] private KeyCode rotateRightKey = KeyCode.X;
        [SerializeField] private KeyCode rotateLeftKey = KeyCode.Z;
        [SerializeField] private bool useMouseRotation = true;
        [SerializeField] private KeyCode mouseRotationKey = KeyCode.Mouse1;

        #endregion

        private CinemachineCamera _virtualCamera;
        private CinemachineFollow _transposer;
        private Transform _cameraTransform;
        private float _zoomPosition = 0; // Value in range (0, 1) used for zoom interpolation
        private Transform _targetFollow;
        private Vector3 _targetOffset;

        #region Properties

        public bool IsFollowingTarget => _targetFollow != null;

        private Vector2 KeyboardInput => useKeyboardInput
            ? new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis))
            : Vector2.zero;

        private Vector2 MouseInput => Input.mousePosition;
        private float ScrollWheelInput => Input.GetAxis(zoomingAxis);
        private Vector2 MouseAxis => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        private int ZoomDirection
        {
            get
            {
                bool zoomIn = Input.GetKey(zoomInKey);
                bool zoomOut = Input.GetKey(zoomOutKey);

                if (zoomIn && zoomOut) return 0;
                if (!zoomIn && zoomOut) return 1;
                if (zoomIn && !zoomOut) return -1;
                return 0;
            }
        }

        private int RotationDirection
        {
            get
            {
                bool rotateRight = Input.GetKey(rotateRightKey);
                bool rotateLeft = Input.GetKey(rotateLeftKey);

                if (rotateLeft && rotateRight) return 0;
                if (rotateLeft && !rotateRight) return -1;
                if (!rotateLeft && rotateRight) return 1;
                return 0;
            }
        }

        #endregion

        private void Awake()
        {
            _virtualCamera = GetComponent<CinemachineCamera>();
            _transposer = _virtualCamera.GetComponent<CinemachineFollow>();
            _cameraTransform = transform;

            if (_transposer == null)
            {
                Debug.LogError("CinemachineCamera requires a Transposer component for this script to work");
                enabled = false;
            }
        }

        private void Update()
        {
            UpdateCamera();
        }

        private void UpdateCamera()
        {
            if (IsFollowingTarget)
            {
                FollowTarget();
            }
            else
            {
                MoveCamera();
            }

            UpdateHeightAndZoom();
            UpdateRotation();
            LimitCameraPosition();
        }

        private void MoveCamera()
        {
            Vector3 desiredMove = Vector3.zero;

            // Keyboard movement
            if (useKeyboardInput)
            {
                desiredMove += new Vector3(KeyboardInput.x, 0, KeyboardInput.y) * keyboardMovementSpeed;
            }

            // Screen edge movement
            if (useScreenEdgeInput)
            {
                Rect leftRect = new Rect(0, 0, screenEdgeBorder, Screen.height);
                Rect rightRect = new Rect(Screen.width - screenEdgeBorder, 0, screenEdgeBorder, Screen.height);
                Rect upRect = new Rect(0, Screen.height - screenEdgeBorder, Screen.width, screenEdgeBorder);
                Rect downRect = new Rect(0, 0, Screen.width, screenEdgeBorder);

                desiredMove.x += leftRect.Contains(MouseInput) ? -1 : rightRect.Contains(MouseInput) ? 1 : 0;
                desiredMove.z += upRect.Contains(MouseInput) ? 1 : downRect.Contains(MouseInput) ? -1 : 0;

                desiredMove *= screenEdgeMovementSpeed;
            }

            // Panning with middle mouse button
            if (usePanning && Input.GetKey(panningKey) && MouseAxis != Vector2.zero)
            {
                desiredMove += new Vector3(-MouseAxis.x, 0, -MouseAxis.y) * panningSpeed;
            }

            // Apply movement
            if (desiredMove != Vector3.zero)
            {
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(0f, _cameraTransform.eulerAngles.y, 0f) * desiredMove;
                _cameraTransform.Translate(desiredMove, Space.World);
            }
        }

        private void UpdateHeightAndZoom()
        {
            // Update zoom position
            if (useScrollWheelZooming)
            {
                _zoomPosition += ScrollWheelInput * Time.deltaTime * scrollWheelZoomSensitivity;
            }

            if (useKeyboardZooming)
            {
                _zoomPosition += ZoomDirection * Time.deltaTime * keyboardZoomSensitivity;
            }

            _zoomPosition = Mathf.Clamp01(_zoomPosition);

            // Calculate target height
            float targetHeight = Mathf.Lerp(minHeight, maxHeight, _zoomPosition);

            if (autoHeight)
            {
                float currentHeight = GetCurrentHeight();
                float difference = targetHeight - currentHeight;
                targetHeight += difference;
            }

            // Update camera height
            Vector3 newPosition = _cameraTransform.position;
            newPosition.y = Mathf.Lerp(newPosition.y, targetHeight, Time.deltaTime * heightDampening);
            _cameraTransform.position = newPosition;

            // Update follow offset for Cinemachine
            if (_transposer != null)
            {
                Vector3 followOffset = _transposer.FollowOffset;
                followOffset.y = -targetHeight; // Invert since Cinemachine uses local offset
                _transposer.FollowOffset = followOffset;
            }
        }

        private void UpdateRotation()
        {
            float rotationAmount = 0f;

            if (useKeyboardRotation)
            {
                rotationAmount += RotationDirection * keyboardRotationSpeed;
            }

            if (useMouseRotation && Input.GetKey(mouseRotationKey))
            {
                rotationAmount += -MouseAxis.x * mouseRotationSpeed;
            }

            if (rotationAmount != 0f)
            {
                _cameraTransform.Rotate(Vector3.up, rotationAmount * Time.deltaTime, Space.World);
            }
        }

        private void FollowTarget()
        {
            if (_targetFollow == null) return;

            Vector3 targetPosition = new Vector3(
                _targetFollow.position.x + _targetOffset.x,
                _cameraTransform.position.y,
                _targetFollow.position.z + _targetOffset.z);

            _cameraTransform.position = Vector3.Lerp(
                _cameraTransform.position,
                targetPosition,
                Time.deltaTime * followingSpeed);
        }

        private void LimitCameraPosition()
        {
            if (!limitMap) return;

            Vector3 position = _cameraTransform.position;
            position.x = Mathf.Clamp(position.x, -limitX, limitX);
            position.z = Mathf.Clamp(position.z, -limitY, limitY);
            _cameraTransform.position = position;
        }

        private float GetCurrentHeight()
        {
            if (Physics.Raycast(_cameraTransform.position, Vector3.down, out RaycastHit hit, groundMask.value))
            {
                return hit.distance;
            }
            return 0f;
        }

        public void SetFollowTarget(Transform target, Vector3 offset = default)
        {
            _targetFollow = target;
            _targetOffset = offset;

            if (_virtualCamera != null)
            {
                _virtualCamera.Follow = target;
                _virtualCamera.LookAt = target;
            }
        }

        public void ClearFollowTarget()
        {
            _targetFollow = null;

            if (_virtualCamera != null)
            {
                _virtualCamera.Follow = null;
                _virtualCamera.LookAt = null;
            }
        }
    }
}
