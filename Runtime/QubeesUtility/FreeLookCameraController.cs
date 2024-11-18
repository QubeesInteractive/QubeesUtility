using System.Collections;
using NaughtyAttributes;
using QubeesUtility.Runtime.QubeesUtility.Extensions;
using Unity.Cinemachine;
using UnityEngine;

namespace QubeesUtility.Runtime.QubeesUtility
{
    public enum ZoomType
    {
        FOV,
        MoveForward,
        LowerY
    }

    public class FreeLookCameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

        [Header("Movement")] [SerializeField] public Vector2 movementClampX;
        [SerializeField] public Vector2 movementClampZ;
        [SerializeField] public bool useKeyboardMovement = true;
        [SerializeField] public bool useMoveCameraWithRightMouseButton;
        [SerializeField] public bool useEdgeScrolling;

        [ShowIf("useEdgeScrolling")] [SerializeField]
        public float edgeScrollSize = 30f;

        [SerializeField] public float moveSpeed = 100f;
        [SerializeField] public float moveLerp = 50f;

        [Header("Rotate")] [SerializeField] public float rotateWithKeyboardSpeed = 120f;
        [SerializeField] public float rotateWithMouseButtonSpeed = 2f;
        [Range(-89, 89)] [SerializeField] public float rotateUpClamp = 45f;
        [SerializeField] public float rotateLerp = 10f;

        [Header("Zoom")] [SerializeField] public float zoomAmount;
        [SerializeField] public float zoomLerpSpeed;
        [SerializeField] public ZoomType zoomType;

        [ShowIf("zoomType", ZoomType.FOV)] [SerializeField]
        public float fovMin;

        [ShowIf("zoomType", ZoomType.FOV)] [SerializeField]
        public float fovMax;

        [ShowIf("zoomType", ZoomType.LowerY)] [SerializeField]
        public float followOffsetMinY;

        [ShowIf("zoomType", ZoomType.LowerY)] [SerializeField]
        public float followOffsetMaxY;

        public bool _canMove = true;
        public bool _canRotateWithKeyboard = true;
        public bool _canRotateWithMouse = true;
        public bool _canZoom = true;

        private void Awake()
        {
            InitMovement();
            InitZoom();
            InitRotation();
            InitShake();
        }

        private void Update()
        {
            if (_canMove)
            {
                if (useKeyboardMovement) HandleMovementWithKeyboard();
                if (useMoveCameraWithRightMouseButton) HandleMovementWithRightClick();
                if (useEdgeScrolling) HandleMovementWithEdgeScrolling();
            }

            if (_canRotateWithKeyboard)
            {
                HandleRotationWithKeyboard();
            }

            if (_canRotateWithMouse)
            {
                HandleRotationWithMouseButton();
            }

            if (_canZoom)
            {
                switch (zoomType)
                {
                    case ZoomType.FOV:
                        HandleCameraZoom_FOV();
                        break;
                    case ZoomType.MoveForward:
                        HandleCameraZoom_MoveForward();
                        break;
                    case ZoomType.LowerY:
                        HandleCameraZoom_LowerY();
                        break;
                }
            }
        }

        public void CanMove(bool status)
        {
            if (!status)
            {
                movementTarget = transform.position;
            }

            _canMove = status;
        }

        public void CanRotate(bool keyboardStatus, bool mouseStatus)
        {
            if (!keyboardStatus && !mouseStatus)
            {
                _appliedYaw = _yaw;
                _appliedPitch = _pitch;
                transform.eulerAngles = new Vector3(_appliedPitch, _appliedYaw, 0f);
            }

            _canRotateWithKeyboard = keyboardStatus;
            _canRotateWithMouse = mouseStatus;
        }

        public void CanZoom(bool status)
        {
            if (!status)
            {
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
                    _followOffset;
            }

            _canZoom = status;
        }

        #region Movement

        private bool _moveCameraWithRightMouseButton;
        private Vector2 _lastMousePosition = Vector2.zero;
        public Vector3 movementTarget;

        private void InitMovement()
        {
            movementTarget = cinemachineVirtualCamera.transform.position;
        }

        public void SetMoveTarget(Vector3 target)
        {
            movementTarget = target.With(y: transform.position.y);
        }

        private void HandleMovementWithKeyboard()
        {
            Vector3 inputDirection = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) inputDirection.z = 1f;
            if (Input.GetKey(KeyCode.S)) inputDirection.z = -1f;
            if (Input.GetKey(KeyCode.A)) inputDirection.x = -1f;
            if (Input.GetKey(KeyCode.D)) inputDirection.x = 1f;

            Vector3 moveDirection = transform.forward * inputDirection.z
                                    + transform.right * inputDirection.x;

            moveDirection.y = 0;

            movementTarget += moveDirection.normalized * (moveSpeed * Time.unscaledDeltaTime);
            movementTarget.x = Mathf.Clamp(movementTarget.x, movementClampX.x, movementClampX.y);
            movementTarget.y = transform.position.y;
            movementTarget.z = Mathf.Clamp(movementTarget.z, movementClampZ.x, movementClampZ.y);

            transform.position = Vector3.Lerp(transform.position, movementTarget, moveLerp * Time.unscaledDeltaTime);
        }

        private void HandleMovementWithEdgeScrolling()
        {
            Vector3 inputDirection = Vector3.zero;

            if (Input.mousePosition.x < edgeScrollSize) inputDirection.x = -1f;
            if (Input.mousePosition.y < edgeScrollSize) inputDirection.z = -1f;
            if (Input.mousePosition.x > Screen.width - edgeScrollSize) inputDirection.x = 1f;
            if (Input.mousePosition.y > Screen.height - edgeScrollSize) inputDirection.z = 1f;

            if (inputDirection.magnitude < Mathf.Epsilon) return;

            Vector3 moveDirection = transform.forward * inputDirection.z
                                    + transform.right * inputDirection.x;

            moveDirection.y = 0;

            Vector3 clampedValue = transform.position + moveDirection.normalized * (moveSpeed * Time.unscaledDeltaTime);
            clampedValue.x = Mathf.Clamp(clampedValue.x, movementClampX.x, movementClampX.y);
            clampedValue.z = Mathf.Clamp(clampedValue.z, movementClampZ.x, movementClampZ.y);

            transform.position = clampedValue;
        }

        private void HandleMovementWithRightClick()
        {
            Vector3 inputDirection = Vector3.zero;

            if (Input.GetMouseButtonDown(1))
            {
                _moveCameraWithRightMouseButton = true;
                _lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(1))
            {
                _moveCameraWithRightMouseButton = false;
            }

            if (_moveCameraWithRightMouseButton)
            {
                Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - _lastMousePosition;
                inputDirection.x = mouseMovementDelta.x;
                inputDirection.z = mouseMovementDelta.y;

                _lastMousePosition = Input.mousePosition;
            }

            if (inputDirection.magnitude < Mathf.Epsilon) return;

            Vector3 moveDirection = transform.forward * inputDirection.z
                                    + transform.right * inputDirection.x;

            moveDirection.y = 0;

            Vector3 clampedValue = transform.position + moveDirection.normalized * (moveSpeed * Time.unscaledDeltaTime);
            clampedValue.x = Mathf.Clamp(clampedValue.x, movementClampX.x, movementClampX.y);
            clampedValue.z = Mathf.Clamp(clampedValue.z, movementClampZ.x, movementClampZ.y);

            transform.position = Vector3.Lerp(transform.position, clampedValue, moveLerp * Time.unscaledDeltaTime);
        }

        #endregion

        #region Zoom

        public Vector3 _followOffset = Vector3.zero;
        public float _targetFov;
        public Vector3 _zoomTarget;
        public float _moveForwardZoomAmount;

        private void InitZoom()
        {
            _targetFov = cinemachineVirtualCamera.m_Lens.FieldOfView;
            _zoomTarget = transform.position;
            _followOffset.y = transform.position.y;
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = _followOffset;
            transform.SetYPosition(0);
        }

        private void HandleCameraZoom_FOV()
        {
            if (Input.mouseScrollDelta.y < 0)
            {
                _targetFov += zoomAmount;
            }

            if (Input.mouseScrollDelta.y > 0)
            {
                _targetFov -= zoomAmount;
            }

            _targetFov = Mathf.Clamp(_targetFov, fovMin, fovMax);

            cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(
                cinemachineVirtualCamera.m_Lens.FieldOfView,
                _targetFov,
                Time.unscaledDeltaTime * zoomLerpSpeed);
        }

        private void HandleCameraZoom_MoveForward()
        {
            Vector3 zoomDir = transform.forward.normalized;

            if (Input.mouseScrollDelta.y < 0)
            {
                _moveForwardZoomAmount -= zoomAmount;
            }

            if (Input.mouseScrollDelta.y > 0)
            {
                _moveForwardZoomAmount += zoomAmount;
            }

            _zoomTarget = zoomDir * _moveForwardZoomAmount;
            _zoomTarget.x = 0;

            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset
                = Vector3.Lerp(
                    cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset,
                    _zoomTarget,
                    zoomLerpSpeed * Time.unscaledDeltaTime);
        }

        private void HandleCameraZoom_LowerY()
        {
            if (Input.mouseScrollDelta.y < 0)
            {
                _followOffset.y += zoomAmount;
            }

            if (Input.mouseScrollDelta.y > 0)
            {
                _followOffset.y -= zoomAmount;
            }

            _followOffset.y = Mathf.Clamp(_followOffset.y, followOffsetMinY, followOffsetMaxY);
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset
                = Vector3.Lerp(
                    cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset,
                    _followOffset,
                    zoomLerpSpeed * Time.unscaledDeltaTime);
        }

        #endregion

        #region Rotation

        public bool _rotateCameraWithRightMouseButton;
        public float _yaw;
        public float _pitch;

        public float _appliedYaw;
        public float _appliedPitch;

        public bool _isRotatingWithMouseButton;
        public bool _isRotatingWithKeyboard;

        private void InitRotation()
        {
            _pitch = transform.eulerAngles.x;
            _yaw = transform.eulerAngles.y;

            _appliedPitch = _pitch;
            _appliedYaw = _yaw;
        }

        private void HandleRotationWithKeyboard()
        {
            if (_isRotatingWithMouseButton) return;
            float rotateDir = 0f;
            if (Input.GetKey(KeyCode.Q)) rotateDir = -1f;
            if (Input.GetKey(KeyCode.E)) rotateDir = 1f;
            _isRotatingWithKeyboard = Mathf.Abs(rotateDir) > 0f;
            if (_isRotatingWithKeyboard)
            {
                transform.eulerAngles +=
                    new Vector3(0, rotateDir * rotateWithKeyboardSpeed * Time.unscaledDeltaTime, 0);
                _yaw = transform.eulerAngles.y;
                _appliedYaw = _yaw;
            }
        }

        private void HandleRotationWithMouseButton()
        {
            if (_isRotatingWithKeyboard) return;
            _isRotatingWithMouseButton = Input.GetMouseButton(2);
            if (_isRotatingWithMouseButton)
            {
                _pitch = Mathf.Clamp(_pitch + Input.GetAxis("Mouse Y") * -rotateWithMouseButtonSpeed, rotateUpClamp,
                    89);
                _yaw -= Input.GetAxis("Mouse X") * -rotateWithMouseButtonSpeed;
            }

            _appliedYaw = Mathf.Lerp(_appliedYaw, _yaw, Time.unscaledDeltaTime * rotateLerp);
            _appliedPitch = Mathf.Lerp(_appliedPitch, _pitch, Time.unscaledDeltaTime * rotateLerp);
            transform.eulerAngles = new Vector3(_appliedPitch, _appliedYaw, 0f);
        }

        #endregion

        #region Shake

        public float _shakeTimer;
        public bool _isShakeCameraActive;

        public void InitShake()
        {
            var cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera
                .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (cinemachineBasicMultiChannelPerlin)
            {
                cinemachineBasicMultiChannelPerlin.AmplitudeGain = 0;
                cinemachineBasicMultiChannelPerlin.FrequencyGain = 0;
            }
        }

        [Button]
        public void ShakeCamera(float intensity = 1, float frequency = 1, float time = 1)
        {
            StartCoroutine(ShakeRoutine());

            IEnumerator ShakeRoutine()
            {
                if (_isShakeCameraActive) yield break;

                _isShakeCameraActive = true;
                var cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera
                    .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cinemachineBasicMultiChannelPerlin.AmplitudeGain = intensity;
                cinemachineBasicMultiChannelPerlin.FrequencyGain = frequency;
                yield return new WaitForSeconds(time);
                cinemachineBasicMultiChannelPerlin.AmplitudeGain = 0;
                cinemachineBasicMultiChannelPerlin.FrequencyGain = 0;
                _isShakeCameraActive = false;
            }
        }

        #endregion
    }
}