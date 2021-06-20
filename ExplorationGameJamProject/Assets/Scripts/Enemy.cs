using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
  [SerializeField] private List<Vector3> respawnPositions;
  [SerializeField] private string memoryArea;

  public GameObject followObject;
  public float speed;
  private Vector3 lookDirection;

  void Update()
  {
    lookDirection = Vector3.Normalize(followObject.transform.position - transform.position);
    transform.Translate(lookDirection * Time.deltaTime * speed);
  }

  public void Respawn()
  {
    int i = Utils.RandomInt(respawnPositions.Count);
    transform.position = respawnPositions[i];
  }
}
