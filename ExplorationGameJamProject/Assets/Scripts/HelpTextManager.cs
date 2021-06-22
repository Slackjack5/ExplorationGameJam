using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HelpTextManager : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI helpText;
  [SerializeField] private string interact = "Press E to pick up.";
  [SerializeField] private string equipCamera = "Right click to equip the camera.";
  [SerializeField] private string takePhoto = "Left click with your camera equipped to take a photo.";
  [SerializeField] private string openInventory = "Press Tab to open the inventory.";
  [SerializeField] private string captureEnemy = "Left click with your camera equipped to fend off Amnesia.";

  public bool IsCaptureEnemyComplete { get; set; }
  public bool IsEquipCameraComplete { get; set; }
  public bool IsInteractComplete { get; set; }
  public bool IsOpenInventoryComplete { get; set; }
  public bool IsTakePhotoComplete { get; set; }
  
  private void Start()
  {
    IsCaptureEnemyComplete = false;
    IsEquipCameraComplete = false;
    IsInteractComplete = false;
    IsOpenInventoryComplete = false;
    IsTakePhotoComplete = false;
  }

  public void RemoveText()
  {
    helpText.text = "";
  }

  public void ShowCaptureEnemyText()
  {
    if (!IsCaptureEnemyComplete)
    {
      helpText.text = captureEnemy;
    }
  }

  public void ShowEquipCameraText()
  {
    if (!IsEquipCameraComplete)
    {
      helpText.text = equipCamera;
    }
  }

  public void ShowInteractText()
  {
    if (!IsInteractComplete)
    {
      helpText.text = interact;
    }
  }

  public void ShowOpenInventoryText()
  {
    if (!IsOpenInventoryComplete)
    {
      helpText.text = openInventory;
    }
  }

  public void ShowTakePhotoText()
  {
    if (!IsTakePhotoComplete)
    {
      helpText.text = takePhoto;
    }
  }
}
