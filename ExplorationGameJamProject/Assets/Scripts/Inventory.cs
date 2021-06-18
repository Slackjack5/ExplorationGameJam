using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
  [SerializeField] private GameObject inventoryPanel;
  [SerializeField] private GameObject sidebar;
  [SerializeField] private GameObject selectedPhotoPanel;
  [SerializeField] private float photoShrinkFactor = 0.07f;
  [SerializeField] private float selectedPhotoShrinkFactor = 0.5f;

  private List<Photo> photos;
  private GameObject selectedPhoto;

  public bool IsOpen {
    get { return inventoryPanel.activeSelf; }
  }

  private void Start()
  {
    photos = new List<Photo>();

    inventoryPanel.SetActive(false);
  }

  public void AddPhoto(Sprite sprite, Vector3 location)
  {
    GameObject photoObject = new GameObject();
    Image image = photoObject.AddComponent<Image>();
    image.sprite = sprite;

    Button button = photoObject.AddComponent<Button>();
    button.onClick.AddListener(() => Select(photoObject));

    RectTransform rectTransform = photoObject.GetComponent<RectTransform>();
    rectTransform.SetParent(sidebar.transform);
    rectTransform.sizeDelta = photoShrinkFactor * new Vector2(sprite.texture.width, sprite.texture.height);

    photos.Add(new Photo(photoObject, location));
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

  private void Select(GameObject photo)
  {
    if (selectedPhoto != null)
    {
      Destroy(selectedPhoto);
    }

    selectedPhoto = Instantiate(photo);
    selectedPhoto.GetComponent<Button>().enabled = false;

    Image image = selectedPhoto.GetComponent<Image>();
    Sprite photoSprite = image.sprite;

    RectTransform rectTransform = selectedPhoto.GetComponent<RectTransform>();
    rectTransform.SetParent(selectedPhotoPanel.transform);
    rectTransform.localPosition = Vector2.zero;
    rectTransform.sizeDelta = selectedPhotoShrinkFactor * new Vector2(photoSprite.texture.width, photoSprite.texture.height);
  }
}
