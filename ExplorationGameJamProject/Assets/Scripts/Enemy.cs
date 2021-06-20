using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
  [SerializeField] private List<Vector3> respawnPositions;

  public GameObject followObject;
  public float speed;
  private bool isActivated;
  private Vector3 lookDirection;

  private void Start()
  {
    isActivated = true;
  }

  void Update()
  {
    if (isActivated)
    {
      lookDirection = Vector3.Normalize(followObject.transform.position - transform.position);
      transform.Translate(lookDirection * Time.deltaTime * speed);
    }
  }

  public void Activate()
  {
    isActivated = true;
  }

  public void Respawn()
  {
    int i = Utils.RandomInt(respawnPositions.Count);
    transform.position = respawnPositions[i];
  }

  public void Suppress()
  {
    isActivated = false;
    Respawn();
  }
}
