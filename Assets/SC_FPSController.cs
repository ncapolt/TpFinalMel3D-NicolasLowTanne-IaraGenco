using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SC_FPSController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    // Head bobbing settings
    public bool useHeadBob = true;
    public float headBobSpeed = 10f;  // Speed of the head bobbing
    public float headBobAmount = 0.05f;  // Amount of head bobbing

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    Vector3 originalCameraPosition;
    float bobCycle = 0f;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Store the original camera position
        originalCameraPosition = playerCamera.transform.localPosition;
    }

    void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        // Apply head bobbing effect
        UpdateCameraPosition(curSpeedX);
    }

    // Head bobbing implementation
    private void UpdateCameraPosition(float speed)
    {
        if (!useHeadBob || !characterController.isGrounded)
        {
            // Reset camera position if head bobbing is not used or player is not grounded
            playerCamera.transform.localPosition = originalCameraPosition;
            return;
        }

        // Calculate head bobbing effect based on speed
        bobCycle += (speed * headBobSpeed) * Time.deltaTime;

        // Apply sinusoidal movement to simulate head bobbing
        float bobbingOffset = Mathf.Sin(bobCycle) * headBobAmount;

        // Apply the offset to the camera position
        Vector3 newCameraPosition = originalCameraPosition;
        newCameraPosition.y += bobbingOffset;
        playerCamera.transform.localPosition = newCameraPosition;
    }
}
