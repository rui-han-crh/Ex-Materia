using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenuBehaviour : MonoBehaviour
{
    [SerializeField]
    private RectTransform slotsTransform;

    [SerializeField]
    private Image largePreviewSprite;

    [SerializeField]
    private TMP_Text previewNameText;

    [SerializeField]
    private TMP_Text previewDescriptionText;

    [SerializeField]
    private Button readButton;

    [SerializeField]
    private GameObject inventorySlotPrefab;

    private InventoryManager inventoryManager;

    private int numberOfSlots = 0;

    private void Awake()
    {
        inventoryManager = InventoryManager.Instance;
    }

    public void OnEnable()
    {
        IEnumerable<InventoryItem> inventoryItems = inventoryManager.InventoryItems;
        Debug.Log($"There were {inventoryItems.Count()} items in inventory");

        while (numberOfSlots < inventoryItems.Count())
        {
            Instantiate(inventorySlotPrefab, slotsTransform);
            numberOfSlots++;
        }

        while (numberOfSlots > inventoryItems.Count())
        {
            Destroy(transform.GetChild(numberOfSlots));
            numberOfSlots--;
        }

        for (int i = 0; i < inventoryItems.Count(); i++)
        {
            InventoryItem item = inventoryItems.ElementAt(i);
            GameObject slot = slotsTransform.GetChild(i).gameObject;
            slot.GetComponent<InventorySlotBehaviour>().SlotImageReference.sprite = item.Sprite;
            slot.GetComponent<Button>().onClick.AddListener(
                () =>
                {
                    largePreviewSprite.sprite = item.Sprite;
                    largePreviewSprite.preserveAspect = true;
                    previewNameText.text = item.ItemName;
                    previewDescriptionText.text = item.Description;

                    if (item is ReadableInventoryItem)
                    {
                        readButton.gameObject.SetActive(true);
                        readButton.onClick.AddListener(() => ScreenObjectManager.Instance.ShowObject(((ReadableInventoryItem)item).ReadableCanvas));
                    } 
                    else
                    {
                        readButton.gameObject.SetActive(false);
                    }
                }
            );
        }
    }
}
