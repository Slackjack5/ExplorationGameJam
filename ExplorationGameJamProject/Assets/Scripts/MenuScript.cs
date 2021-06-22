using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
  public void LoadScene(string scene)
  {
    if (scene == "MainScene")
    {
      Vector3 offmap = new Vector3(9999999, 9999999, 9999999);
      Shader.SetGlobalVector("_Point1", offmap);
      Shader.SetGlobalFloat("_Point1Time", -10000);
      Shader.SetGlobalVector("_Point2", offmap);
      Shader.SetGlobalFloat("_Point2Time", -10000);
      Shader.SetGlobalVector("_Point3", offmap);
      Shader.SetGlobalFloat("_Point3Time", -10000);
      Shader.SetGlobalVector("_PhotoPoint", offmap);
      Shader.SetGlobalFloat("_PhotoPointTime", -10000);
    }
    SceneManager.LoadScene(scene);
  }

  public void Exit()
  {
    Application.Quit();
  }
}
