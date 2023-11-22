using System.Collections;
using UnityEngine;
[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController;
    [SerializeField] float walkingSpeed;
    [SerializeField] float runningSpeed;
    [SerializeField] float sprintSpeed;
    [Range(0, 1)]
    [SerializeField] float runThreshold = .7f;
    [Range(0, 1)]
    [SerializeField] float deadZone = .1f;
    [Range(0, 50)]
    [SerializeField] float acceleration = 5;
    [Range(0,50)]
    [SerializeField] float deceleration = 15;
    [Tooltip("Stamina/s")]
    [SerializeField] float sprintStaminaCost = 15;
    float currentSpeed = 0;
    new Camera camera;
    Vector3 movement;
    Vector3 direction = Vector3.forward;
    PlayerController playerController;
    Animator animator;
    float dropSpeed = 0;
    float targetSpeed = 0;
    bool wasSprinting = false;
    bool isDecelerating = false;
    [SerializeField] float turnSpeed = 500;
    bool isGrounded = false;
    [SerializeField] float jumpSpeed = 5;

    public bool IsSprinting { get; private set; } = false;
    Vector2 previousMovement = Vector2.zero;
    public float Gravity
    {
        get
        {
            dropSpeed += Physics.gravity.y * Time.deltaTime;
            return dropSpeed * Time.deltaTime;
        }
    }
    void Awake()
    {
        characterController = GetComponentInChildren<CharacterController>();
        camera = Camera.main;
        playerController = GetComponent<PlayerController>();
        animator = GetComponentInChildren<Animator>();

        animator = GetComponentInChildren<Animator>();
        if (animator == null)
            throw new MissingComponentException("Animator missing on player");
    }

    void Update()
    {
        HandleMovement();

        characterController.Move(new Vector3(0, Gravity, 0));
        isGrounded = characterController.isGrounded;
        if (characterController.isGrounded)
            dropSpeed = -1f;
        movement = Quaternion.Euler(0, camera.transform.eulerAngles.y, 0) * movement; //handle camera rotation
        Quaternion movementForward;

        if (direction.magnitude > 0)
        {
            movementForward = Quaternion.LookRotation(direction, Vector3.up);
            characterController.transform.rotation = Quaternion.RotateTowards(characterController.transform.rotation, movementForward, turnSpeed * Time.deltaTime);
        }

        characterController.Move(movement);
    }

    void HandleMovement()
    {
        Vector2 movementInput = playerController.Move; 
        float movementMagnitude = movementInput.magnitude;
        IsSprinting = playerController.IsSprinting && movementMagnitude >= runThreshold;
        animator.SetBool("IsSprinting", IsSprinting);
        if (movementMagnitude >= deadZone) 
        {
            animator.SetBool("IsMoving", true);
            direction = Quaternion.Euler(0, camera.transform.eulerAngles.y, 0) * new Vector3(movementInput.x, 0, movementInput.y);


            if (IsSprinting)
            {
                targetSpeed = targetSpeed > sprintSpeed ? sprintSpeed : targetSpeed + Time.deltaTime * acceleration;
                targetSpeed = targetSpeed > sprintSpeed ? sprintSpeed : targetSpeed;
            }
            else if (movementMagnitude >= runThreshold)
            {
                targetSpeed = runningSpeed;
                animator.SetFloat("MovementSpeedMultiplier", runningSpeed / walkingSpeed);
            }
            else
            {
                targetSpeed = walkingSpeed;
                animator.SetFloat("MovementSpeedMultiplier", 1);
            }

            animator.SetFloat("MovementX", 0);
            animator.SetFloat("MovementY", 1);
        }
        else
        {
            targetSpeed = 0;
            animator.SetBool("IsMoving", false);
        }

        if (wasSprinting && !IsSprinting) StartCoroutine(Decelerate());
        wasSprinting = IsSprinting;
        if (!isDecelerating)
        {
            currentSpeed = targetSpeed;
            movementInput = currentSpeed * Time.deltaTime * movementInput.normalized;
        }
        else if (movementMagnitude < deadZone)
        {
            movementInput = currentSpeed * Time.deltaTime * previousMovement;
        }
        else
            movementInput = currentSpeed * Time.deltaTime * movementInput.normalized;
        movement.x = movementInput.x;
        movement.z = movementInput.y;
        if (movementMagnitude >= deadZone)
            previousMovement = movementInput.normalized;
    }

    IEnumerator Decelerate()
    {
        isDecelerating = true;
        while (targetSpeed < currentSpeed)
        {
            yield return null;
            currentSpeed -= Time.deltaTime * deceleration;
        }
        currentSpeed = targetSpeed;
        isDecelerating = false;
    }
    void OnJump()
    {
        if (isGrounded)
        {
            dropSpeed = jumpSpeed;
        }
    }
}