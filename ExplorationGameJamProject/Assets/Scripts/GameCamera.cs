using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
  // Unity Editor fields
  [SerializeField] private GameObject cameraCanvas;

  // Private properties
  private Inventory inventory;

  // Start is called before the first frame update
  void Start()
  {
    inventory = GetComponent<Inventory>();

    cameraCanvas.SetActive(false);
  }

  public void TakePhoto()
  {
    if (cameraCanvas.activeSelf && !inventory.IsOpen)
    {
      // Disable the camera UI so that it doesn't appear in screenshot
      cameraCanvas.SetActive(false);
      StartCoroutine(CaptureFrame());
    }
  }

  public void Toggle()
  {
    cameraCanvas.SetActive(!cameraCanvas.activeSelf);
  }

  private IEnumerator CaptureFrame()
  {
    yield return new WaitForEndOfFrame();

    Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
    Sprite photo = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    inventory.AddPhoto(photo);
    cameraCanvas.SetActive(true);
  }
}
