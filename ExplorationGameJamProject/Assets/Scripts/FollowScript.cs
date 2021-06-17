using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
  public GameObject followObject;
  public float speed;
  private Vector3 lookDirection;
  void Update()
  {
    lookDirection = Vector3.Normalize(followObject.transform.position - transform.position);
    transform.Translate(lookDirection * Time.deltaTime * speed);
  }
}
