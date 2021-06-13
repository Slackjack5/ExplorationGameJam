using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Unity Editor fields
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float baseSpeed = 6f;
    [SerializeField] private float lookSensitivity = 0.2f;

    // Private properties
    private float cameraRotationX;
    private CharacterController characterController;
    private float lookInputX;
    private float lookInputY;
    private float moveInputX;
    private float moveInputZ;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Look
        float targetHorizontalLook = lookSensitivity * lookInputX;
        transform.Rotate(Vector3.up * targetHorizontalLook);

        float targetVerticalLook = lookSensitivity * lookInputY;
        cameraRotationX -= targetVerticalLook;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
    }

    private void FixedUpdate()
    {
        // Move
        float targetVelocityX = baseSpeed * moveInputX * Time.fixedDeltaTime;
        float targetVelocityZ = baseSpeed * moveInputZ * Time.fixedDeltaTime;
        Vector3 targetVelocity = transform.right * targetVelocityX + transform.forward * targetVelocityZ;
        characterController.Move(targetVelocity);
    }

    public void OnLook(InputValue value)
    {
        Vector2 lookVector = value.Get<Vector2>();
        lookInputX = lookVector.x;
        lookInputY = lookVector.y;
    }

    public void OnMove(InputValue value)
    {
        Vector2 motionVector = value.Get<Vector2>();
        moveInputX = motionVector.x;
        moveInputZ = motionVector.y;
    }
}
