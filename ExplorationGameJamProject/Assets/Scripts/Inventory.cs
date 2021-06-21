using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
  [SerializeField] private List<Transform> memoryAreas;

  // Private properties
  private List<Photo> photos;
  private GameObject selectedPhoto;
  private Vector3 selectedPhotoPos;
  private bool photoHasBeenSelected;
  private HashSet<Vector3> requiredLocations;

  public int CurrentCapacity { get; private set; }

  public bool HasSpace
  {
    get { return photos.Count < CurrentCapacity; }
  }

  public bool IsOpen
  {
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
    photoHasBeenSelected = false;

    inventoryPanel.SetActive(false);

    // Initialize the set of required locations to win
    requiredLocations = new HashSet<Vector3>();
    foreach (Transform transform in memoryAreas)
    {
      requiredLocations.Add(transform.position);
    }
  }

  public void AddPhoto(Sprite sprite, Vector3 location)
  {
    GameObject photoObject = new GameObject();
    bool isMemory = location != Vector3.zero;
    Photo photo = new Photo(photoObject, location, isMemory);

    Image image = photoObject.AddComponent<Image>();
    image.sprite = sprite;

    Button button = photoObject.AddComponent<Button>();
    button.onClick.AddListener(() => Select(photo));

    RectTransform rectTransform = photoObject.GetComponent<RectTransform>();
    rectTransform.SetParent(sidebar.transform);
    rectTransform.sizeDelta = photoShrinkFactor * new Vector2(sprite.texture.width, sprite.texture.height);

    photos.Add(photo);

    CheckWinCondition();
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
      photoHasBeenSelected = false;
    }
    else
    {
      if (photoHasBeenSelected)
      {
        Shader.SetGlobalVector("_PhotoPoint", selectedPhotoPos);
        Shader.SetGlobalFloat("_PhotoPointTime", Time.time);
      }
      Cursor.lockState = CursorLockMode.Locked;
    }
  }

  private void CheckWinCondition()
  {
    HashSet<Vector3> collectedLocations = new HashSet<Vector3>();
    foreach (Photo photo in photos)
    {
      collectedLocations.Add(photo.location);
    }

    if (collectedLocations.Count > 0 && collectedLocations.SetEquals(requiredLocations))
    {
      SceneManager.LoadScene("YouWin");
    }
  }

  private void Select(Photo photo)
  {
    if (selectedPhoto != null)
    {
      Destroy(selectedPhoto);
    }

    photoHasBeenSelected = true;
    selectedPhotoPos = photo.location;
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
