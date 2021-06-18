using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
  // Unity Editor fields
  [SerializeField] private GameObject cameraCanvas;
  [SerializeField] private LayerMask whatIsMemory;
  [SerializeField] private float maxMemoryCaptureDistance = 5f;

  // Private properties
  private Inventory inventory;

  // Start is called before the first frame update
  void Start()
  {
    inventory = GetComponent<Inventory>();

    cameraCanvas.SetActive(false);
  }

  public void TakePhoto(Ray lookRay)
  {
    if (cameraCanvas.activeSelf && !inventory.IsOpen)
    {
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

  private Vector3 GetLocation(Ray lookRay)
  {
    if (Physics.Raycast(lookRay, out RaycastHit hit, maxMemoryCaptureDistance, whatIsMemory, QueryTriggerInteraction.Collide))
    {
      return hit.transform.position;
    }

    return Vector3.zero;
  }
}
