using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldPlayerController : MonoBehaviour
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
    [SerializeField] float sensitivity = 1f;
    [SerializeField] float cameraClampAngle = 90f;

    CharacterController controller;
    
    Vector3 currentVelocity = Vector3.zero;
    float xRotation = 0f;
    float ySpeed = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        MouseLook();
        Move();
    }

    void Move()
    {
        float vertical = 0;
        float horizontal = 0;

        if (canControl)
        {
            vertical = Input.GetAxisRaw("Vertical");
            horizontal = Input.GetAxisRaw("Horizontal");
        }   

        Vector3 inputDirection = (transform.forward * vertical + transform.right * horizontal).normalized;

        float desiredSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * runMultiplier : speed;
        Vector3 desiredVelocity = inputDirection * desiredSpeed;

        currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, acceleration * Time.deltaTime);

        if (controller.isGrounded)
        {
            if ((Input.GetKey(KeyCode.Space) && holdSpace) || Input.GetKeyDown(KeyCode.Space) && canControl)
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

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -cameraClampAngle, cameraClampAngle);

        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void ChangeSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity;
    }
}
