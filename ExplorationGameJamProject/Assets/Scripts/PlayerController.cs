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
  [SerializeField] private float lerp = 0.1f;

  // Private properties
  private float cameraRotationX;
  private CharacterController characterController;
  private GameCamera gameCamera;
  private GameObject lastHighlightedObject;
  private float lookInputX;
  private float lookInputY;
  private float moveInputX;
  private float moveInputZ;
  private Vector3 currentVelocity;
  private Material originalMaterial;

  // Shader stuff
  public float shaderDelay;
  private float shaderTimer;
  private int nextPoint;

  // Start is called before the first frame update
  void Start()
  {
    nextPoint = 0;
    shaderTimer = Time.time + shaderDelay;
    characterController = GetComponent<CharacterController>();
    gameCamera = GetComponent<GameCamera>();

    Cursor.lockState = CursorLockMode.Locked;
  }

  // Update is called once per frame
  void Update()
  {
    Shader.SetGlobalVector("_PlayerPos", transform.position);
    if (shaderTimer <= Time.time)
    {
      switch (nextPoint)
      {
        case 0:
          Shader.SetGlobalVector("_Point1", transform.position);
          Shader.SetGlobalFloat("_Point1Time", Time.time); break;
        case 1:
          Shader.SetGlobalVector("_Point2", transform.position);
          Shader.SetGlobalFloat("_Point2Time", Time.time); break;
        case 2:
          Shader.SetGlobalVector("_Point3", transform.position);
          Shader.SetGlobalFloat("_Point3Time", Time.time); break;
      }
      if (nextPoint < 2)
      {
        nextPoint++;
      }
      else
      {
        nextPoint = 0;
      }
      shaderTimer = Time.time + shaderDelay;
    }

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
    currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, lerp);
    characterController.Move(currentVelocity);
  }

  public void OnFire()
  {
    gameCamera.TakePicture();
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
  
  public void OnSecondaryFire()
  {
    gameCamera.Toggle();
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
