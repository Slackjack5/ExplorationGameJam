using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
  // Unity Editor fields
  [SerializeField] private GameObject cameraCanvas;

  // Private properties
  private int pictureCount;

  // Constants
  private const string pathPrefix = "Screenshots/";

  // Start is called before the first frame update
  void Start()
  {
    cameraCanvas.SetActive(false);
  }

  public void TakePicture()
  {
    if (cameraCanvas.activeSelf)
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

    ScreenCapture.CaptureScreenshot(pathPrefix + pictureCount + ".png");
    pictureCount++;
    cameraCanvas.SetActive(true);
  }
}
