using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class PlayerController : MonoBehaviour
{
  // Unity Editor fields
  [SerializeField] private Camera playerCamera;
  [SerializeField] private LayerMask whatIsInteractable;
  [SerializeField] private string memoryArea = "Memory Area";
  [SerializeField] private float baseSpeed = 6f;
  [SerializeField] private float maxInteractDistance = 1f;
  [SerializeField] private float lerp = 0.1f;
  [SerializeField] private List<Transform> respawnPositions;
  [SerializeField] private GameObject cameraPivot;
  [SerializeField] private Animator myAnimator;

  // Private properties
  private float cameraRotationX;
  private GameObject lastHighlightedObject;
  public GameObject pauseMenu;
  public Volume volume;
  public GameObject enemy;
  private float lookInputX;
  private float lookInputY;
  private float moveInputX;
  private float moveInputZ;
  private Vector3 currentVelocity;
  private Bloom bloom;
  private float deathTime;
  private float respawnTime;
  private bool hasJustDied;
  private bool respawning;
  private ColorAdjustments colorAdjustments;
  private InputAction action;

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
    // Load in bloom and colorAdjustment component
    volume.profile.TryGet<Bloom>(out bloom);
    volume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
    // Insure no shader nonsense at start of scene
    Vector3 offmap = new Vector3(9999999, 9999999, 9999999);
    Shader.SetGlobalVector("_Point1", offmap);
    Shader.SetGlobalFloat("_Point1Time", -10000);
    Shader.SetGlobalVector("_Point2", offmap);
    Shader.SetGlobalFloat("_Point2Time", -10000);
    Shader.SetGlobalVector("_Point3", offmap);
    Shader.SetGlobalFloat("_Point3Time", -10000);

    // Initialize components
    characterController = GetComponent<CharacterController>();
    gameCamera = GetComponent<GameCamera>();
    inventory = GetComponent<Inventory>();

    nextPoint = 0;
    shaderTimer = Time.time + shaderDelay;
    Time.timeScale = 1;
    Cursor.lockState = CursorLockMode.Locked;

    cameraPivot.SetActive(false);
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

    // Change bloom if enemy is close
    Vector2 playerXZ = new Vector2(transform.position.x, transform.position.z);
    Vector2 enemyXZ = new Vector2(enemy.transform.position.x, enemy.transform.position.z);
    float enemyDistance = Mathf.Abs(Vector2.Distance(playerXZ, enemyXZ));
    if (enemyDistance < 5)
    {
      bloom.intensity.value = Mathf.Max(0.1f, (1 - enemyDistance / 5f) * 5f);
    }
    else
    {
      bloom.intensity.value = 0.1f;
    }

    // Do fade to red if dying
    if (Time.time - deathTime < 2 && hasJustDied)
    {
      Color redFade = new Color(1, 1 - (Time.time - deathTime) / 2, 1 - (Time.time - deathTime) / 2, 1);
      float contrast = (Time.time - deathTime) / 2 * -100;
      colorAdjustments.contrast.value = contrast;
      colorAdjustments.colorFilter.value = redFade;
    }
    else if (Time.time - deathTime > 2 && hasJustDied)
    {
      int i = Utils.RandomInt(respawnPositions.Count);
      transform.position = respawnPositions[i].position;

      inventory.LosePhoto();
      enemy.GetComponent<Enemy>().Respawn();

      hasJustDied = false;
      respawning = true;
    }

    // If not dead and colorAdjustments are set to dead state, fade back in.
    if (!hasJustDied && deathTime != 0 && respawning == true)
    {
      respawnTime = Time.time;
      respawning = false;
    }

    if (Time.time - respawnTime < 1 && !hasJustDied && respawnTime != 0)
    {
      Color redFade = new Color(1, (Time.time - respawnTime) / 1, (Time.time - respawnTime) / 1, 1);
      float contrast = (1 - (Time.time - respawnTime) / 1) * -100;
      colorAdjustments.contrast.value = contrast;
      colorAdjustments.colorFilter.value = redFade;
    }
    else if (Time.time - respawnTime > 1 && !hasJustDied && respawnTime != 0)
    {
      colorAdjustments.contrast.value = 0;
      colorAdjustments.colorFilter.value = new Color(1, 1, 1, 1);
    }

    // Look
    if (IsGameActive())
    {
      float targetHorizontalLook = Settings.sensitivity * lookInputX;
      transform.Rotate(Vector3.up * targetHorizontalLook);

      float targetVerticalLook = Settings.sensitivity * lookInputY;
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

    if (!IsGameActive())
    {
      targetVelocity = Vector3.zero;
    }

    currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, lerp);
    characterController.Move(currentVelocity);

    //Footstep audio
    if (characterController.velocity.magnitude >= .1f && playingFootstep == false)
    {
      AkSoundEngine.PostEvent("Play_Footsteps", this.gameObject);
      StartCoroutine(NextFootstep());
      playingFootstep = true;
    }

  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.layer == LayerMask.NameToLayer(memoryArea))
    {
      enemy.GetComponent<Enemy>().Suppress();
    }
    else if (other.gameObject.name == enemy.name)
    {
      Respawn();
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.gameObject.layer == LayerMask.NameToLayer(memoryArea))
    {
      enemy.GetComponent<Enemy>().Activate();
    }
  }

  IEnumerator NextFootstep()
  {
    yield return new WaitForSeconds(.6f);
    playingFootstep = false;
  }

  public void OnPause()
  {
    pauseMenu.SetActive(!pauseMenu.activeSelf);
    Cursor.lockState = pauseMenu.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
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
    if (IsSeeingInteractable(out RaycastHit hit) && IsGameActive())
    {
      Interactable interactable = hit.transform.gameObject.GetComponent<Interactable>();
      if (interactable.IsActive)
      {
        inventory.IncreaseCapacity();
        hit.transform.gameObject.GetComponent<Interactable>().Interact();
      }
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
    if (!pauseMenu.activeSelf)
    {
      inventory.Toggle();
    }
  }

  public void OnSecondaryFire()
  {
    if (IsGameActive())
    {
      if (!gameCamera.IsEquipped)
      {
        // Animate equipping of camera
        cameraPivot.SetActive(true);
        myAnimator.SetBool("Camera Toggled", true);
      }
      else
      {
        gameCamera.Toggle();
      }
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

  private bool IsGameActive()
  {
    return !pauseMenu.activeSelf && !inventory.IsOpen;
  }

  private bool IsSeeingInteractable(out RaycastHit hit)
  {
    return Physics.Raycast(Utils.GetLookRay(playerCamera), out hit, maxInteractDistance, whatIsInteractable);
  }

  private void Respawn()
  {
    deathTime = Time.time;
    hasJustDied = true;
  }
}
