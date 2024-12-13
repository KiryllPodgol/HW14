using UnityEngine;
using UnityEngine.InputSystem;

namespace Scrtipt
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Movement Settings")] public float MoveSpeed = 5.0f;
        public float SprintSpeed = 7.0f;
        public float RotationSpeed = 10.0f;
        public float JumpHeight = 1.5f;
        public float Gravity = -9.81f;

        [Header("Camera Settings")] public Transform CameraTransform;
        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;

        private CharacterController _controller;
        private Animator _animator;
        private Vector3 _velocity;
        private Vector3 _moveDirection;
        private float _targetSpeed;
        private float _verticalRotation;

        private InputAsset _input;

        private static readonly int SpeedParam = Animator.StringToHash("Speed");
        private static readonly int JumpParam = Animator.StringToHash("Jump");

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _input = new InputAsset();

            if (CameraTransform == null)
            {
                Debug.LogError("CameraTransform is not assigned.");
            }
        }

        private void OnEnable()
        {
            _input.Gameplay.Enable();

            _input.Gameplay.Move.performed += OnMovePerformed;
            _input.Gameplay.Move.canceled += OnMoveCanceled;

            _input.Gameplay.Look.performed += OnLookPerformed;

            _input.Gameplay.Jump.performed += OnJumpPerformed;
            _input.Gameplay.Jump.canceled += OnJumpCanceled;

            _input.Gameplay.Sprint.performed += OnSprintPerformed;
            _input.Gameplay.Sprint.canceled += OnSprintCanceled;
        }

        private void OnDisable()
        {
            _input.Gameplay.Move.performed -= OnMovePerformed;
            _input.Gameplay.Move.canceled -= OnMoveCanceled;

            _input.Gameplay.Look.performed -= OnLookPerformed;

            _input.Gameplay.Jump.performed -= OnJumpPerformed;
            _input.Gameplay.Jump.canceled -= OnJumpCanceled;

            _input.Gameplay.Sprint.performed -= OnSprintPerformed;
            _input.Gameplay.Sprint.canceled -= OnSprintCanceled;

            _input.Gameplay.Disable();
        }

        private void Update()
        {
            HandleMovement();
            HandleJumpAndGravity();
            UpdateAnimator();
        }

        private void HandleMovement()
        {
            float targetSpeed = _input.Gameplay.Sprint.IsPressed() ? SprintSpeed : MoveSpeed;
            Vector3 inputDirection = new Vector3(_input.Gameplay.Move.ReadValue<Vector2>().x, 0,
                _input.Gameplay.Move.ReadValue<Vector2>().y).normalized;

            if (inputDirection.magnitude > 0.1f)
            {
                _moveDirection = Quaternion.Euler(0, CameraTransform.eulerAngles.y, 0) * inputDirection;
                Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);
            }

            Vector3 move = _moveDirection * targetSpeed;
            _controller.Move(move * Time.deltaTime);
        }

        private void HandleJumpAndGravity()
        {
            if (_velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            if (_input.Gameplay.Jump.WasPressedThisFrame())
            {
                _velocity.y = Mathf.Sqrt(2 * JumpHeight * -Gravity);
            }

            _velocity.y += Gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }

        private void UpdateAnimator()
        {
            float speed = _controller.velocity.magnitude;
            _animator.SetFloat(SpeedParam, speed / SprintSpeed);

            if (_input.Gameplay.Jump.WasPressedThisFrame())
            {
                _animator.SetTrigger(JumpParam);
            }
        }

        private void LateUpdate()
        {
            HandleCameraRotation();
        }

        private void HandleCameraRotation()
        {
            Vector2 lookInput = _input.Gameplay.Look.ReadValue<Vector2>();
            float mouseX = lookInput.x;
            float mouseY = lookInput.y;

            _verticalRotation -= mouseY;
            _verticalRotation = Mathf.Clamp(_verticalRotation, BottomClamp, TopClamp);

            CameraTransform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
            transform.Rotate(Vector3.up * mouseX);
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            // Placeholder if specific actions are needed during movement
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            // Placeholder if specific actions are needed when movement stops
        }

        private void OnLookPerformed(InputAction.CallbackContext context)
        {
            // Placeholder for look actions
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            // Placeholder for jump actions
        }

        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            // Placeholder for jump cancel actions
        }

        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            // Placeholder for sprint actions
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            // Placeholder for sprint cancel actions
        }
    }
}
