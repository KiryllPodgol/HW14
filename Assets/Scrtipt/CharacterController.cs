using UnityEngine;

namespace Scrtipt
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;

        [Header("Movement Settings")]
        public bool analogMovement;
        public float moveSpeed = 5f;
        public float sprintMultiplier = 1.5f; // Multiplier for sprint speed
        public float gravity = -9.81f;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

        private InputAsset _input;
        private CharacterController _controller; // Reference to CharacterController
        private Vector3 _velocity; // Gravity velocity
        private Animator _animator; // Reference to Animator

        private void Awake()
        {
            // Создаем экземпляр сгенерированного InputAsset
            _input = new InputAsset();
            _controller = GetComponent<CharacterController>(); // Initialize CharacterController
            _animator = GetComponent<Animator>(); // Initialize Animator
        }

        private void OnEnable()
        {
            // Включаем карту действий Gameplay
            _input.Gameplay.Enable();

            // Подписываемся на действия
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
            // Отписываемся от действий
            _input.Gameplay.Move.performed -= OnMovePerformed;
            _input.Gameplay.Move.canceled -= OnMoveCanceled;

            _input.Gameplay.Look.performed -= OnLookPerformed;

            _input.Gameplay.Jump.performed -= OnJumpPerformed;
            _input.Gameplay.Jump.canceled -= OnJumpCanceled;

            _input.Gameplay.Sprint.performed -= OnSprintPerformed;
            _input.Gameplay.Sprint.canceled -= OnSprintCanceled;

            // Отключаем карту действий Gameplay
            _input.Gameplay.Disable();
        }

        private void OnMovePerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            move = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            move = Vector2.zero;
        }

        private void OnLookPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (cursorInputForLook)
                look = context.ReadValue<Vector2>();
        }

        private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            jump = true;
        }

        private void OnJumpCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            jump = false;
        }

        private void OnSprintPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            sprint = true;
        }

        private void OnSprintCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            sprint = false;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }

        private void Update()
        {
            if (_controller != null)
            {
                // Movement logic
                Vector3 moveDirection = new Vector3(move.x, 0, move.y);
                if (moveDirection.magnitude > 0.1f)
                {
                    moveDirection = transform.TransformDirection(moveDirection);

                    float currentSpeed = moveSpeed;
                    if (sprint)
                    {
                        currentSpeed *= sprintMultiplier;
                    }

                    _controller.Move(moveDirection * currentSpeed * Time.deltaTime);
                }

                // Gravity logic
                if (!_controller.isGrounded)
                {
                    _velocity.y += gravity * Time.deltaTime;
                }
                else if (jump)
                {
                    _velocity.y = Mathf.Sqrt(-2f * gravity * moveSpeed); // Jump force
                    jump = false; // Reset jump state
                }
                else
                {
                    _velocity.y = -2f; // Keep grounded
                }

                _controller.Move(_velocity * Time.deltaTime);
                UpdateAnimator();
            }
        }

        private void UpdateAnimator()
        {
            // Speed parameter for blend tree (idle, walk, run)
            float speed = move.magnitude * (sprint ? sprintMultiplier : 1f);

            // Set motionSpeed parameter (adjusts blend tree smoothly)
            _animator.SetFloat("motionSpeed", speed);

            // Set grounded parameter
            _animator.SetBool("Grounded", _controller.isGrounded);

            // Set jump trigger (for jump animation)
            if (jump)
            {
                _animator.SetTrigger("Jump");
            }
        }
    }
}