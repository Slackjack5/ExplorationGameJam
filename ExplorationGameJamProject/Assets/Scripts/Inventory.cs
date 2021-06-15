using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
  [SerializeField] private GameObject inventoryPanel;
  [SerializeField] private float photoShrinkFactor = 0.25f;

  private List<Sprite> photos;

  private void Start()
  {
    photos = new List<Sprite>();
  }

  public void AddPhoto(Sprite photo)
  {
    photos.Add(photo);

    GameObject newObject = new GameObject();
    Image newImage = newObject.AddComponent<Image>();
    newImage.sprite = photo;

    RectTransform rectTransform = newObject.GetComponent<RectTransform>();
    rectTransform.SetParent(inventoryPanel.transform);
    rectTransform.localPosition = new Vector2(0, 0);
    rectTransform.sizeDelta = photoShrinkFactor * new Vector2(photo.texture.width, photo.texture.height);
  }
}
