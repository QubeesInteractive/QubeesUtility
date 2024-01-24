using Cinemachine;
using NaughtyAttributes;
using QubeesUtility.Runtime.QubeesUtility.Extensions;
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

        [Header("Movement")]
        [SerializeField] private Vector2 movementClampX;
        [SerializeField] private Vector2 movementClampZ;
        [SerializeField] private bool useKeyboardMovement = true;
        [SerializeField] private bool useMoveCameraWithRightMouseButton;
        [SerializeField] private bool useEdgeScrolling;
        [ShowIf("useEdgeScrolling")]
        [SerializeField] private float edgeScrollSize = 30f;
        [SerializeField] private float moveSpeed = 100f;
        [SerializeField] private float moveLerp = 50f;

        [Header("Rotate")]
        [SerializeField] private bool rotateWithKeyboard;
        [ShowIf("rotateWithKeyboard")]
        [SerializeField] private float rotateWithKeyboardSpeed = 120f;
        [SerializeField] private bool rotateWithMouseButton;
        [ShowIf("rotateWithMouseButton")]
        [SerializeField] private float rotateWithMouseButtonSpeed = 2f;
        [Range(-89,89)]
        [SerializeField] private float rotateUpClamp = 45f;
        [SerializeField] private float rotateLerp = 10f;

        [Header("Zoom")]
        [SerializeField] private float zoomAmount;
        [SerializeField] private float zoomLerpSpeed;
        [SerializeField] private ZoomType zoomType;
        [ShowIf("zoomType", ZoomType.FOV)]
        [SerializeField] private float fovMin;
        [ShowIf("zoomType", ZoomType.FOV)]
        [SerializeField] private float fovMax;
    
        [ShowIf("zoomType", ZoomType.MoveForward)]
        [SerializeField] private float followOffsetMin;
        [ShowIf("zoomType", ZoomType.MoveForward)]
        [SerializeField] private float followOffsetMax;
        [ShowIf("zoomType", ZoomType.LowerY)]
        [SerializeField] private float followOffsetMinY;
        [ShowIf("zoomType", ZoomType.LowerY)]
        [SerializeField] private float followOffsetMaxY;


        private void Awake()
        {
            InitMovement();
            InitZoom();
            InitRotation();
        }
    
        private void Update()
        {
            if (useKeyboardMovement) HandleMovementWithKeyboard();
            if (useMoveCameraWithRightMouseButton) HandleMovementWithRightClick();
            if (useEdgeScrolling) HandleMovementWithEdgeScrolling();
        
            if (rotateWithKeyboard) HandleRotationWithKeyboard();
            if (rotateWithMouseButton) HandleRotationWithMouseButton();

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

        #region Movement
    
        private bool _moveCameraWithRightMouseButton;
        private Vector2 _lastMousePosition = Vector2.zero;
        // public Transform movementTarget;
        public Vector3 movementTarget;
        private void InitMovement()
        {
            movementTarget = cinemachineVirtualCamera.transform.position;
        }
        
        private void HandleMovementWithKeyboard()
        {
            Vector3 inputDirection = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) inputDirection.z = 1f;
            if (Input.GetKey(KeyCode.S)) inputDirection.z = -1f;
            if (Input.GetKey(KeyCode.A)) inputDirection.x = -1f;
            if (Input.GetKey(KeyCode.D)) inputDirection.x = 1f;

            // if (inputDirection.magnitude < Mathf.Epsilon) return;
            
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
            // transform.position = Vector3.Lerp(transform.position, clampedValue, moveLerp * Time.deltaTime);
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

        private Vector3 _followOffset = Vector3.zero;
        private float _targetFov;
        private Vector3 _zoomTarget;
        private float _moveForwardZoomAmount;

        private void InitZoom()
        {
            // _followOffset = cinemachineVirtualCamera
            //     .GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
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
                // _zoomTargetPosition -= zoomDir * zoomAmount;
                _moveForwardZoomAmount -=  zoomAmount;
            }

            if (Input.mouseScrollDelta.y > 0)
            {
                // _zoomTargetPosition += zoomDir * zoomAmount;
                _moveForwardZoomAmount +=  zoomAmount;
            }
            _zoomTarget = zoomDir * _moveForwardZoomAmount;
            _zoomTarget.x = 0;
            // if (_followOffset.magnitude < followOffsetMin)
            // {
            //     _followOffset = zoomDir * followOffsetMin;
            // }
            // if (_followOffset.magnitude > followOffsetMax)
            // {
            //     _followOffset = zoomDir * followOffsetMax;
            // }

            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset
            // transform.position
                = Vector3.Lerp(
                    cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset,
                    // transform.position,
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

        private bool _rotateCameraWithRightMouseButton;
        private float _yaw;
        private float _pitch;

        private float _appliedYaw;
        private float _appliedPitch;
        
        private void InitRotation()
        {
            _pitch = transform.eulerAngles.x;
            _yaw = transform.eulerAngles.y;
            
            _appliedPitch = _pitch;
            _appliedYaw = _yaw;
        }
        private void HandleRotationWithKeyboard()
        {
            float rotateDir = 0f;
            if (Input.GetKey(KeyCode.Q)) rotateDir = 1f;
            if (Input.GetKey(KeyCode.E)) rotateDir = -1f;

            transform.eulerAngles += new Vector3(0, rotateDir * rotateWithKeyboardSpeed * Time.unscaledDeltaTime, 0);
        }

        private void HandleRotationWithMouseButton()
        {
            if(Input.GetMouseButton(2)) {
                _pitch = Mathf.Clamp(_pitch + Input.GetAxis("Mouse Y") * -rotateWithMouseButtonSpeed, rotateUpClamp, 89);
                _yaw -= Input.GetAxis("Mouse X") * -rotateWithMouseButtonSpeed;
            }
            _appliedYaw = Mathf.Lerp(_appliedYaw, _yaw, Time.unscaledDeltaTime * rotateLerp);
            _appliedPitch = Mathf.Lerp(_appliedPitch, _pitch, Time.unscaledDeltaTime * rotateLerp);
            transform.eulerAngles = new Vector3(_appliedPitch, _appliedYaw, 0f);

        }
        #endregion

    }
}