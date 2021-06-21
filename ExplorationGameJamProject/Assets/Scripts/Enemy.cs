using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Enemy : MonoBehaviour
{
  [SerializeField] private List<Transform> respawnPositions;
  [SerializeField] private float aggroAcceleration;
  [SerializeField] private float maxSpeed;
  
  public GameObject followObject;
  public Material enemyMaterial;
  private float aggro = 1;
  private bool isAccelerating;
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
    if (isActivated)
    {
      if (aggro < maxSpeed && !isAccelerating)
      {
        isAccelerating = true;
        StartCoroutine(Accelerate());
      }

      visualEffect.SetFloat("SpawnRate", aggro);
      enemyMaterial.SetFloat("_WobbleSpeed", aggro / 10 + (aggro - 1) * 0.2f);
      lookDirection = Vector3.Normalize(followObject.transform.position - transform.position);
      transform.Translate(lookDirection * Time.deltaTime * aggro);
    }
    else
    {
      visualEffect.SetFloat("SpawnRate", 0);
      enemyMaterial.SetFloat("_WobbleSpeed", 0);
      aggro = 1;
    }
  }

  public void Activate()
  {
    isActivated = true;
  }

  public void Respawn()
  {
    aggro = 1;

    int i = Utils.RandomInt(respawnPositions.Count);
    transform.position = respawnPositions[i].position;
  }

  public void Suppress()
  {
    isActivated = false;
    Respawn();
  }

  private IEnumerator Accelerate()
  {
    yield return new WaitForSeconds(1f);

    aggro += aggroAcceleration;
    isAccelerating = false;
  }
}
