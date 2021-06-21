using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
  [SerializeField] private Animator myAnimator;
  [SerializeField] private GameObject blackScreen;
  [SerializeField] private GameCamera gameCamera;

  public void ShowBlackScreen()
  {
    blackScreen.SetActive(true);
    StartCoroutine(Reset());
  }

  private IEnumerator Reset()
  {
    yield return new WaitForSeconds(0.1f);

    blackScreen.SetActive(false);
    myAnimator.SetBool("Camera Toggled", false);

    gameCamera.Toggle();
    gameObject.SetActive(false);
  }
}
