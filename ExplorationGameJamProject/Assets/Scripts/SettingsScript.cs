using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
  public GameObject sensitivity;
  public GameObject music;
  public GameObject sound;
  public void Start()
  {
    sensitivity.GetComponent<Slider>().value = Settings.sensitivity;
    music.GetComponent<Slider>().value = Settings.musicVolume;
    sound.GetComponent<Slider>().value = Settings.soundVolume;
  }
  public void OnSensitivityChange(float value)
  {
    Settings.sensitivity = value;
  }

  public void OnMusicChange(float value)
  {
    Settings.musicVolume = value;
  }

  public void OnSoundChange(float value)
  {
    Settings.soundVolume = value;
  }
}
