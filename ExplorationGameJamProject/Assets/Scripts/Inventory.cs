using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
  [SerializeField] private GameObject inventoryPanel;
  [SerializeField] private GameObject sidebar;
  [SerializeField] private float photoShrinkFactor = 0.07f;

  private List<Sprite> photos;

  public bool IsOpen {
    get { return inventoryPanel.activeSelf; }
  }

  private void Start()
  {
    photos = new List<Sprite>();

    inventoryPanel.SetActive(false);
  }

  public void AddPhoto(Sprite photo)
  {
    GameObject newObject = new GameObject();
    Image newImage = newObject.AddComponent<Image>();
    newImage.sprite = photo;

    RectTransform rectTransform = newObject.GetComponent<RectTransform>();
    rectTransform.SetParent(sidebar.transform);
    rectTransform.sizeDelta = photoShrinkFactor * new Vector2(photo.texture.width, photo.texture.height);

    photos.Add(photo);
  }

  public void Toggle()
  {
    inventoryPanel.SetActive(!inventoryPanel.activeSelf);

    if (IsOpen)
    {
      Cursor.lockState = CursorLockMode.None;
    }
    else
    {
      Cursor.lockState = CursorLockMode.Locked;
    }
  }
}
