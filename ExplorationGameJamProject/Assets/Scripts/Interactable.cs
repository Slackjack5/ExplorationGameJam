using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
  [SerializeField] private float respawnTime = 60f;

  public void Interact()
  {
    GetComponent<MeshRenderer>().enabled = false;
    GetComponent<Light>().enabled = false;
    StartCoroutine(Respawn());
  }

  private IEnumerator Respawn()
  {
    yield return new WaitForSeconds(respawnTime);

    GetComponent<MeshRenderer>().enabled = true;
    GetComponent<Light>().enabled = true;
  }
}
