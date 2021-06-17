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
  [SerializeField] private float baseSpeed = 6f;
  [SerializeField] private float lookSensitivity = 0.2f;
  [SerializeField] private float maxInteractDistance = 1f;
  [SerializeField] private float lerp = 0.1f;

  // Private properties
  private float cameraRotationX;
  private GameObject lastHighlightedObject;
  public GameObject pauseMenu;
  private float lookInputX;
  private float lookInputY;
  private float moveInputX;
  private float moveInputZ;
  private Vector3 currentVelocity;

  // Components
  private CharacterController characterController;
  private GameCamera gameCamera;
  private Inventory inventory;

  // Shader stuff
  public float shaderDelay;
  private float shaderTimer;
  private int nextPoint;

    //Audio Timer
    private bool playingFootstep = false;

  // Start is called before the first frame update
  void Start()
  {
    // Initialize components
    characterController = GetComponent<CharacterController>();
    gameCamera = GetComponent<GameCamera>();
    inventory = GetComponent<Inventory>();

    nextPoint = 0;
    shaderTimer = Time.time + shaderDelay;

    Time.timeScale = 1;
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
    if (!pauseMenu.activeSelf && !inventory.IsOpen)
    {
      float targetHorizontalLook = lookSensitivity * lookInputX;
      transform.Rotate(Vector3.up * targetHorizontalLook);

      float targetVerticalLook = lookSensitivity * lookInputY;
      cameraRotationX -= targetVerticalLook;
      cameraRotationX = Mathf.Clamp(cameraRotationX, -90f, 90f);
      playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
    }

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

    if (pauseMenu.activeSelf || inventory.IsOpen)
    {
      targetVelocity = Vector3.zero;
    }

    currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, lerp);
    characterController.Move(currentVelocity);

        //Footstep audio
        if(characterController.velocity.magnitude >= .1f && playingFootstep==false)
        {
            AkSoundEngine.PostEvent("Play_Footsteps", this.gameObject);
            StartCoroutine(NextFootstep());
            playingFootstep = true;
        }

  }

    IEnumerator NextFootstep()
    {
        yield return new WaitForSeconds(.6f);
        print("footstep");
        playingFootstep = false;
    }

        public void OnPause()
  {
    pauseMenu.SetActive(!pauseMenu.activeSelf);
    Cursor.lockState = pauseMenu.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
    Cursor.visible = pauseMenu.activeSelf;
    Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
  }


  public void OnFire()
  {
    if (!pauseMenu.activeSelf)
    {
      gameCamera.TakePhoto();
    }
  }

  public void OnInteract()
  {
    if (IsSeeingInteractable(out RaycastHit hit) && pauseMenu.activeSelf)
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

  public void OnOpenInventory()
  {
    inventory.Toggle();
  }
  
  public void OnSecondaryFire()
  {
    if (!pauseMenu.activeSelf && !inventory.IsOpen)
    {
      gameCamera.Toggle();
    }
  }

  private void ClearHighlight()
  {
    if (lastHighlightedObject != null)
    {
      lastHighlightedObject.GetComponent<Outline>().enabled = false;
      lastHighlightedObject = null;
    }
  }

  private void Highlight(RaycastHit hit)
  {
    GameObject gameObject = hit.transform.gameObject;
    if (lastHighlightedObject != gameObject)
    {
      ClearHighlight();
      gameObject.GetComponent<Outline>().enabled = true;
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