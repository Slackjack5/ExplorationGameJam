using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
  /// <summary>
  /// Returns a random integer from 0 to (max - 1), both included.
  /// </summary>
  public static int RandomInt(int max)
  {
    return Mathf.FloorToInt(Random.value * (max - 1));
  }
}
