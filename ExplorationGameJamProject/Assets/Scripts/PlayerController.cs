using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Unity Editor fields
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask whatIsInteractable;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private float baseSpeed = 6f;
    [SerializeField] private float lookSensitivity = 0.2f;
    [SerializeField] private float maxInteractDistance = 1f;

    // Private properties
    private float cameraRotationX;
    private CharacterController characterController;
    private GameObject lastHighlightedObject;
    private float lookInputX;
    private float lookInputY;
    private float moveInputX;
    private float moveInputZ;
    private Material originalMaterial;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
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

        // Highlight interactable objects that are looked at
        if (IsSeeingInteractable(out RaycastHit hit))
        {
            Highlight(hit);
        }
        else
        {
            ClearHighlight();
        }
    }

    private void FixedUpdate()
    {
        // Move
        float targetVelocityX = baseSpeed * moveInputX * Time.fixedDeltaTime;
        float targetVelocityZ = baseSpeed * moveInputZ * Time.fixedDeltaTime;
        Vector3 targetVelocity = transform.right * targetVelocityX + transform.forward * targetVelocityZ;
        characterController.Move(targetVelocity);
    }

    public void OnInteract()
    {
        if (IsSeeingInteractable(out RaycastHit hit))
        {
            Debug.Log("Interacted with " + hit.transform.name);
            Destroy(hit.transform.gameObject);
        }
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

    private void ClearHighlight()
    {
        if (lastHighlightedObject != null)
        {
            lastHighlightedObject.GetComponent<MeshRenderer>().sharedMaterial = originalMaterial;
            lastHighlightedObject = null;
        }
    }

    private void Highlight(RaycastHit hit)
    {
        GameObject gameObject = hit.transform.gameObject;
        if (lastHighlightedObject != gameObject)
        {
            ClearHighlight();
            originalMaterial = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
            gameObject.GetComponent<MeshRenderer>().sharedMaterial = highlightMaterial;
            lastHighlightedObject = gameObject;
        }
    }

    private bool IsSeeingInteractable(out RaycastHit hit)
    {
        Vector3 viewportCenterPoint = new Vector3(0.5f, 0.5f);
        Ray ray = playerCamera.ViewportPointToRay(viewportCenterPoint);
        return Physics.Raycast(ray, out hit, maxInteractDistance, whatIsInteractable);
    }
}
