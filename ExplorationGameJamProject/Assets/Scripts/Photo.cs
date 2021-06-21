using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photo
{
  public GameObject photoObject { get; }
  public Vector3 location { get; }
  public bool isMemory { get; }

  public Photo(GameObject photoObject, Vector3 location, bool isMemory)
  {
    this.photoObject = photoObject;
    this.location = location;
    this.isMemory = isMemory;
  }
}
