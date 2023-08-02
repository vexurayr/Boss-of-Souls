using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensitivityX;
    [SerializeField] private float mouseSensitivityY;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float gravity;
    [SerializeField] [Range(0.0f, 0.5f)] private float moveSmoothTime;
    [SerializeField] [Range(0.0f, 0.5f)] private float mouseSmoothTime;
    [SerializeField] private AnimationCurve jumpFallOff;
    [SerializeField] private float jumpMultiplier;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    // Left-click to perform a light attack
    [SerializeField] private KeyCode lightAttackKey = KeyCode.Mouse0;
    // Right-click to perform a heavy attack
    [SerializeField] private KeyCode heavyAttackKey = KeyCode.Mouse1;
    [SerializeField] private Weapon leftHandAttack;
    [SerializeField] private Weapon rightHandAttack;
    [SerializeField] private ParticleSystem leftHandParticles;
    [SerializeField] private ParticleSystem rightHandParticles;

    [SerializeField] private SurvivalUI survivalUI;
    private Health health;

    private bool isRightHandAttackNext = true;
    private bool isMovementLocked = false;
    private bool isCameraMovementLocked = false;
    private bool areInputsRegistered = true;
    private bool isInSecondPhase = false;

    private bool isCursorLocked = true;
    private float cameraPitch = 0.0f;
    private float velocityY = 0.0f;
    private bool isJumping;
    CharacterController playerController = null;

    private Vector2 currentDirection = Vector2.zero;
    private Vector2 currentDirVelocity = Vector2.zero;

    private Vector2 currentMouseDelta = Vector2.zero;
    private Vector2 currentMouseDeltaVelocity = Vector2.zero;

    #endregion Variables

    #region MonoBehaviours
    private void Start()
    {
        // Lets the character controller handle movement and collision, applies the value to this component
        playerController = GetComponent<CharacterController>();

        GameManager.instance.SetCurrentPlayerController(this);

        if (isCursorLocked)
        {
            // Locks the cursor to the center of the screen
            Cursor.lockState = CursorLockMode.Locked;

            // Makes the cursor invisible
            Cursor.visible = false;
        }

        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (!isCameraMovementLocked)
        {
            UpdateMouseLook();
        }
        if (!isMovementLocked)
        {
            UpdateMovement();
        }
        if (areInputsRegistered)
        {
            RefreshHealthUI();

            if (Input.GetKeyDown(lightAttackKey))
            {
                bool isSuccess = false;

                if (isRightHandAttackNext)
                {
                    isSuccess = rightHandAttack.PrimaryAction();
                }
                else
                {
                    isSuccess = leftHandAttack.PrimaryAction();
                }

                if (isSuccess)
                {
                    isRightHandAttackNext = !isRightHandAttackNext;
                }
                else if (isRightHandAttackNext)
                {
                    leftHandAttack.PrimaryAction();
                }
                else
                {
                    rightHandAttack.PrimaryAction();
                }
            }
            else if (Input.GetKeyDown(heavyAttackKey))
            {
                leftHandAttack.SecondaryAction();
                rightHandAttack.SecondaryAction();
            }
        }
    }

    #endregion MonoBehaviours

    #region KeyboardAndMouse
    private void UpdateMouseLook()
    {
        // Saves the x and y position of the mouse on the screen
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // Smooths camera movement
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        // Saves the camera's vertical rotation as the inverse of mouse movement
        cameraPitch -= currentMouseDelta.y * mouseSensitivityY;

        // Guarentees the camera does not rotate beyond looking straight up or straight down
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

        // Rotates the camera vertically
        playerCamera.transform.localEulerAngles = Vector3.right * cameraPitch;

        // Rotates the parent object left and right based on the mouse's x (horizontal) position
        this.gameObject.transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivityX);
    }

    private void UpdateMovement()
    {
        Vector3 playerVelocity = Vector3.zero;

        // Storing the axis
        Vector2 targetDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Normalize the vector so every direction is cappped at the same speed
        targetDirection.Normalize();

        // Eases into movement so it's not instantaneous
        currentDirection = Vector2.SmoothDamp(currentDirection, targetDirection, ref currentDirVelocity, moveSmoothTime);

        // Reset the rate the player falls if they are touching the ground
        if (playerController.isGrounded)
        {
            velocityY = 0.0f;
        }

        // The downward acceleration
        velocityY += gravity * Time.deltaTime;

        // Sets the player's speed using the vectors scaled by their axis
        if (Input.GetKey(sprintKey))
        {
            playerVelocity = (transform.forward * currentDirection.y + transform.right * currentDirection.x)
                * sprintSpeed + Vector3.up * velocityY;
        }
        else
        {
            playerVelocity = (transform.forward * currentDirection.y + transform.right * currentDirection.x)
                * walkSpeed + Vector3.up * velocityY;
        }

        if (targetDirection != Vector2.zero)
        {
            if (!AudioManager.instance.IsSoundAlreadyPlaying("Walking"))
            {
                AudioManager.instance.PlayLoopingSound("Walking", transform);
            }

            AudioManager.instance.RefreshAudioTransform("Walking", transform);
        }
        else
        {
            AudioManager.instance.StopSound("Walking");
        }

        // The character controller will do its thing
        playerController.Move(playerVelocity * Time.deltaTime);

        JumpInput();
    }

    private void JumpInput()
    {
        // Player can jump
        if (Input.GetKeyDown(jumpKey) && !isJumping)
        {
            isJumping = true;
            StartCoroutine(JumpEvent());
        }
    }

    private IEnumerator JumpEvent()
    {
        playerController.slopeLimit = 90.0f;
        float timeInAir = 0.0f;

        do
        {
            float jumpForce = jumpFallOff.Evaluate(timeInAir);
            playerController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
            timeInAir += Time.deltaTime;
            yield return null;
            // Stops the player from going any higher if they hit a ceiling
        } while (!playerController.isGrounded && playerController.collisionFlags != CollisionFlags.Above);

        isJumping = false;
        playerController.slopeLimit = 45.0f;
    }

    #endregion KeyboardAndMouse

    #region UI
    public void RefreshHealthUI()
    {
        survivalUI.RefreshHealthUI(health.GetCurrentValue(), health.GetMaxValue());

        if (health.GetCurrentValue() <= health.GetMaxValue() / 2 && !isInSecondPhase)
        {
            isInSecondPhase = true;

            leftHandParticles.Play();
            rightHandParticles.Play();

            leftHandAttack.AddDamageMultiplier(2.8f);
            rightHandAttack.AddDamageMultiplier(2.8f);

            walkSpeed += 10;
            sprintSpeed += 10;

            jumpMultiplier += 15;
        }
    }

    #endregion UI

    #region GetSet
    public Camera GetPlayerCamera()
    {
        return playerCamera;
    }

    public float GetCameraFOV()
    {
        return playerCamera.fieldOfView;
    }

    #endregion GetSet

    #region HelperFunctions
    public void UseMouseToNavigate()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isCameraMovementLocked = true;
    }

    public void UseMouseToObserve()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isCameraMovementLocked = false;
    }

    public void SetAreInputsRegistered(bool newState)
    {
        areInputsRegistered = newState;
    }

    #endregion HelperFunctions
}