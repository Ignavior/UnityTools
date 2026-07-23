using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 4f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float runMultiplier = 1.5f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 1f;
    [SerializeField] bool holdSpace = true;
    public bool canControl = true;

    [Space(10)]

    [Header("Camera")]
    [SerializeField] Transform cameraHolder;
    [SerializeField] public float sensitivity = 0.1f;
    [SerializeField] float cameraClampAngle = 90f;

    [Header("Input")] 
    [SerializeField] InputActionReference moveInput; 
    [SerializeField] InputActionReference lookInput; 
    [SerializeField] InputActionReference jumpInput; 
    [SerializeField] InputActionReference sprintInput;

    CharacterController controller;
    
    Vector3 currentVelocity = Vector3.zero;
    float xRotation = 0f;
    float ySpeed = 0f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        moveInput.action.Enable();
        lookInput.action.Enable();
        jumpInput.action.Enable();
        sprintInput.action.Enable();
    }

    void OnDisable()
    {
        moveInput.action.Disable();
        lookInput.action.Disable();
        jumpInput.action.Disable();
        sprintInput.action.Disable();
    }

    void Update()
    {
        MouseLook();
        Move();
    }

    void Move()
    {
        Vector2 input = canControl
            ? moveInput.action.ReadValue<Vector2>()
            : Vector2.zero;

        Vector3 inputDirection = (transform.forward * input.y + transform.right * input.x).normalized;

        float desiredSpeed = sprintInput.action.IsPressed() ? speed * runMultiplier : speed;
        
        Vector3 desiredVelocity = inputDirection * desiredSpeed;

        currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, acceleration * Time.deltaTime);

        if (controller.isGrounded)
        {
            bool jumpPressed = holdSpace
                ? jumpInput.action.IsPressed()
                : jumpInput.action.WasPressedThisFrame();

            if (jumpPressed && canControl)
                ySpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
            else
                ySpeed = -2f;
        }
        else
        {
            ySpeed += gravity * Time.deltaTime;
        }

        currentVelocity.y = ySpeed;
        controller.Move(currentVelocity * Time.deltaTime);
    }

    void MouseLook()
    {
        if (!canControl)
            return;

        Vector2 look = lookInput.action.ReadValue<Vector2>();

        float mouseX = look.x * sensitivity;
        float mouseY = look.y * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -cameraClampAngle, cameraClampAngle);

        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
