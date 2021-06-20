using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
  /// <summary>
  /// Returns a ray from the center of the viewport.
  /// </summary>
  public static Ray GetLookRay(Camera camera)
  {
    Vector3 viewportCenterPoint = new Vector3(0.5f, 0.5f);
    return camera.ViewportPointToRay(viewportCenterPoint);
  }

  /// <summary>
  /// Returns a random integer from 0 to (max - 1), both included.
  /// </summary>
  public static int RandomInt(int max)
  {
    return Mathf.FloorToInt(Random.value * (max - 1));
  }
}
