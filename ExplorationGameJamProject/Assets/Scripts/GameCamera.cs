using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameCamera : MonoBehaviour
{
  // Unity Editor fields
  [SerializeField] private Camera playerCamera;
  [SerializeField] private GameObject cameraCanvas;
  [SerializeField] private GameObject invalidPhotoPanel;
  [SerializeField] private GameObject validPhotoPanel;
  [SerializeField] private GameObject checkmarkCanvas;
  [SerializeField] private GameObject helpCanvas;
  [SerializeField] private Enemy enemy;
  [SerializeField] private LayerMask whatIsEnemy;
  [SerializeField] private LayerMask whatIsMemory;
  [SerializeField] private TextMeshProUGUI photoCounterText;
  [SerializeField] private GameObject errorText;
  [SerializeField] private float errorTime = 5f;
  [SerializeField] private float maxMemoryCaptureDistance = 5f;

  // Private properties
  private Inventory inventory;

  public bool IsEquipped
  {
    get { return cameraCanvas.activeSelf; }
  }

  public bool IsSeeingEnemy
  {
    get { return Physics.Raycast(Utils.GetLookRay(playerCamera), out RaycastHit hit, maxMemoryCaptureDistance, whatIsEnemy); }
  }

  public bool IsValidPhoto
  {
    get { return validPhotoPanel.activeSelf; }
  }

  // Start is called before the first frame update
  void Start()
  {
    inventory = GetComponent<Inventory>();

    cameraCanvas.SetActive(false);
    checkmarkCanvas.SetActive(false);
    errorText.SetActive(false);
    validPhotoPanel.SetActive(false);
    invalidPhotoPanel.SetActive(true);
  }

  private void Update()
  {
    photoCounterText.text = FormatCounter();

    Ray lookRay = Utils.GetLookRay(playerCamera);
    if (Physics.Raycast(lookRay, out RaycastHit hit, maxMemoryCaptureDistance, whatIsEnemy) || Physics.Raycast(lookRay, out hit, maxMemoryCaptureDistance, whatIsMemory))
    {
      validPhotoPanel.SetActive(true);
      invalidPhotoPanel.SetActive(false);
    }
    else
    {
      validPhotoPanel.SetActive(false);
      invalidPhotoPanel.SetActive(true);
    }
  }

  public void TakePhoto()
  {
    if (cameraCanvas.activeSelf && !inventory.IsOpen && inventory.HasSpace)
    {
      if (IsValidPhoto)
      {
        errorText.SetActive(false);

        Ray lookRay = Utils.GetLookRay(playerCamera);
        if (Physics.Raycast(lookRay, out RaycastHit hit, maxMemoryCaptureDistance, whatIsEnemy))
        {
          enemy.Respawn();
        }

        // Disable the camera and help text UI so that it doesn't appear in screenshot
        cameraCanvas.SetActive(false);
        helpCanvas.SetActive(false);

        Vector3 location = GetLocation(lookRay);
        if (location != Vector3.zero)
        {
          checkmarkCanvas.SetActive(true);
        }

        StartCoroutine(CaptureFrame(lookRay));
      }
      else
      {
        errorText.SetActive(true);
        StartCoroutine(ShowError());
      }
    }
  }

  public void Toggle()
  {
    cameraCanvas.SetActive(!cameraCanvas.activeSelf);
  }

  private IEnumerator CaptureFrame(Ray lookRay)
  {
    yield return new WaitForEndOfFrame();

    Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    inventory.AddPhoto(sprite, GetLocation(lookRay));

    cameraCanvas.SetActive(true);
    helpCanvas.SetActive(true);
    checkmarkCanvas.SetActive(false);
  }

  private string FormatCounter()
  {
    return inventory.PhotoCount + " / " + inventory.CurrentCapacity;
  }

  private Vector3 GetLocation(Ray lookRay)
  {
    if (Physics.Raycast(lookRay, out RaycastHit hit, maxMemoryCaptureDistance, whatIsMemory, QueryTriggerInteraction.Collide))
    {
      return hit.transform.position;
    }

    // Use the zero vector to indicate that the photo is not of a memory area
    return Vector3.zero;
  }

  private IEnumerator ShowError()
  {
    yield return new WaitForSeconds(errorTime);

    errorText.SetActive(false);
  }
}
