using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
  // Unity Editor fields
  [SerializeField] private GameObject inventoryPanel;
  [SerializeField] private GameObject sidebar;
  [SerializeField] private GameObject selectedPhotoPanel;
  [SerializeField] private float photoShrinkFactor = 0.07f;
  [SerializeField] private float selectedPhotoShrinkFactor = 0.5f;
  [SerializeField] private int initialCapacity = 3;

  // Private properties
  private List<Photo> photos;
  private GameObject selectedPhoto;

  public int CurrentCapacity { get; private set; }

  public bool HasSpace
  {
    get { return photos.Count < CurrentCapacity; }
  }

  public bool IsOpen {
    get { return inventoryPanel.activeSelf; }
  }

  public int PhotoCount
  {
    get { return photos.Count; }
  }

  private void Start()
  {
    CurrentCapacity = initialCapacity;
    photos = new List<Photo>();

    inventoryPanel.SetActive(false);
  }

  public void AddPhoto(Sprite sprite, Vector3 location)
  {
    GameObject photoObject = new GameObject();
    Photo photo = new Photo(photoObject, location);

    Image image = photoObject.AddComponent<Image>();
    image.sprite = sprite;

    Button button = photoObject.AddComponent<Button>();
    button.onClick.AddListener(() => Select(photo));

    RectTransform rectTransform = photoObject.GetComponent<RectTransform>();
    rectTransform.SetParent(sidebar.transform);
    rectTransform.sizeDelta = photoShrinkFactor * new Vector2(sprite.texture.width, sprite.texture.height);

    photos.Add(photo);
  }

  public void IncreaseCapacity()
  {
    CurrentCapacity++;
  }

  public void LosePhoto()
  {
    if (photos.Count > 0)
    {
      int i = Utils.RandomInt(photos.Count);
      Photo photo = photos[i];
      photos.Remove(photo);
      Destroy(photo.photoObject);
    }
    
    if (selectedPhoto != null)
    {
      Destroy(selectedPhoto);
      selectedPhoto = null;
    }
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

  private void Select(Photo photo)
  {
    if (selectedPhoto != null)
    {
      Destroy(selectedPhoto);
    }

    selectedPhoto = Instantiate(photo.photoObject);
    selectedPhoto.GetComponent<Button>().enabled = false;

    Image image = selectedPhoto.GetComponent<Image>();
    Sprite photoSprite = image.sprite;

    RectTransform rectTransform = selectedPhoto.GetComponent<RectTransform>();
    rectTransform.SetParent(selectedPhotoPanel.transform);
    rectTransform.localPosition = Vector2.zero;
    rectTransform.sizeDelta = selectedPhotoShrinkFactor * new Vector2(photoSprite.texture.width, photoSprite.texture.height);
  }
}
