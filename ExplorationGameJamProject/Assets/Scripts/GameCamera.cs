using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameCamera : MonoBehaviour
{
  // Unity Editor fields
  [SerializeField] private GameObject cameraCanvas;
  [SerializeField] private Enemy enemy;
  [SerializeField] private LayerMask whatIsEnemy;
  [SerializeField] private LayerMask whatIsMemory;
  [SerializeField] private TextMeshProUGUI photoCounterText;
  [SerializeField] private float maxMemoryCaptureDistance = 5f;

  // Private properties
  private Inventory inventory;

  // Start is called before the first frame update
  void Start()
  {
    inventory = GetComponent<Inventory>();

    cameraCanvas.SetActive(false);
  }

  private void Update()
  {
    photoCounterText.text = FormatCounter();
  }

  public void TakePhoto(Ray lookRay)
  {
    if (cameraCanvas.activeSelf && !inventory.IsOpen && inventory.HasSpace)
    {
      if (Physics.Raycast(lookRay, out RaycastHit hit, maxMemoryCaptureDistance, whatIsEnemy))
      {
        enemy.Respawn();
      }

      // Disable the camera UI so that it doesn't appear in screenshot
      cameraCanvas.SetActive(false);
      StartCoroutine(CaptureFrame(lookRay));
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
}
