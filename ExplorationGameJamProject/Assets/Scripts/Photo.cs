using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photo
{
  public GameObject photoObject { get; }
  public Vector3 location { get; }

  public Photo(GameObject photoObject, Vector3 location)
  {
    this.photoObject = photoObject;
    this.location = location;
  }
}
