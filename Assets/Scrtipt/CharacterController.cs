using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimplePlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    private CharacterController controller;
    private InputAsset inputAsset;

    private Vector2 inputDirection;
    private Vector3 velocity;
    private float currentSpeed;

    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDFreeFall;
    private int animIDMotionSpeed;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputAsset = new InputAsset();
        AssignAnimationIDs();
        currentSpeed = walkSpeed;
    }

    private void OnEnable()
    {
        inputAsset.Gameplay.Enable();
        inputAsset.Gameplay.Move.performed += OnMove;
        inputAsset.Gameplay.Move.canceled += OnMoveCanceled;
        inputAsset.Gameplay.Sprint.started += OnSprintStarted;
        inputAsset.Gameplay.Sprint.canceled += OnSprintCanceled;
        inputAsset.Gameplay.Jump.started += OnJumpStarted;
    }

    private void OnDisable()
    {
        inputAsset.Gameplay.Move.performed -= OnMove;
        inputAsset.Gameplay.Move.canceled -= OnMoveCanceled;
        inputAsset.Gameplay.Sprint.started -= OnSprintStarted;
        inputAsset.Gameplay.Sprint.canceled -= OnSprintCanceled;
        inputAsset.Gameplay.Jump.started -= OnJumpStarted;
        inputAsset.Gameplay.Disable();
    }

    private void Update()
    {
        HandleMovement();
        ApplyGravity();
        UpdateAnimator();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        inputDirection = Vector2.zero;
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        currentSpeed = sprintSpeed;  
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        currentSpeed = runSpeed;  
    }

    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        if (controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(-2f * gravity * 2f);
        }
    }

    private void HandleMovement()
    {
        
        Vector3 moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y).normalized;

      
        if (moveDirection.sqrMagnitude > 0.01f)
        {
           
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;
        float speedPercent = currentSpeed / sprintSpeed;
        animator.SetFloat(animIDSpeed, speedPercent, 0.1f, Time.deltaTime);
        animator.SetFloat(animIDMotionSpeed, speedPercent, 0.1f, Time.deltaTime);
        animator.SetBool(animIDGrounded, controller.isGrounded);
        animator.SetBool(animIDJump, !controller.isGrounded && velocity.y > 0); 
        animator.SetBool(animIDFreeFall, !controller.isGrounded && velocity.y < 0);
    }


    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }
}
