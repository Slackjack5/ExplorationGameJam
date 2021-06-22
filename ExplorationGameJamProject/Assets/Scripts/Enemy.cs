using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Enemy : MonoBehaviour
{
  [SerializeField] private Inventory inventory;
  [SerializeField] private List<Transform> respawnPositions;
  [SerializeField] private float aggroAcceleration = 0.5f;
  
  public GameObject followObject;
  public Material enemyMaterial;
  public float aggro = 1f;
  private bool isActivated;
  private Vector3 lookDirection;
  private VisualEffect visualEffect;

  private void Start()
  {
    visualEffect = GetComponent<VisualEffect>();
    visualEffect.SetFloat("SpawnRate", aggro);
    enemyMaterial.SetFloat("_WobbleSpeed", aggro / 10 + (aggro - 1) * 0.2f);
    isActivated = true;
  }

  void Update()
  {
    aggro = 1f + aggroAcceleration * inventory.GetUniqueCount();

    if (isActivated)
    {
      visualEffect.SetFloat("SpawnRate", aggro);
      enemyMaterial.SetFloat("_WobbleSpeed", aggro / 10 + (aggro - 1) * 0.2f);
      lookDirection = Vector3.Normalize(followObject.transform.position - transform.position);
      transform.Translate(lookDirection * Time.deltaTime * aggro);
    }
    else
    {
      visualEffect.SetFloat("SpawnRate", 0);
      enemyMaterial.SetFloat("_WobbleSpeed", 0);
    }
  }

  public void Activate()
  {
    isActivated = true;
  }

  public void Respawn()
  {
    int i = Utils.RandomInt(respawnPositions.Count);
    transform.position = respawnPositions[i].position;
  }

  public void Suppress()
  {
    isActivated = false;
    Respawn();
  }
}
