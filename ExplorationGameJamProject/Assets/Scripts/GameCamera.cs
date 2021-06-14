using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField] private GameObject cameraCanvas;

    // Start is called before the first frame update
    void Start()
    {
        cameraCanvas.SetActive(false);
    }

    public void Toggle()
    {
        cameraCanvas.SetActive(!cameraCanvas.activeSelf);
    }
}
