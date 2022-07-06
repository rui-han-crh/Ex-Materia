using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemsDatabase : MonoBehaviour
{
    [SerializeField]
    public InventoryItem[] items;

    private Dictionary<string, InventoryItem> nameItemMapping = new Dictionary<string,InventoryItem>();

    public InventoryItem this[string itemName] 
    {
        get 
        {
            try
            {
                return nameItemMapping[itemName];
            }
            catch (KeyNotFoundException)
            {
                Debug.LogError($"There was no item with the name {itemName} registered. " +
                    $"Make sure you are using the FILE NAME and not the item name");
                return null;
            }
        }
    }

    private void Awake()
    {
        foreach (InventoryItem item in items)
        {
            nameItemMapping.Add(item.name, item);
        }
    }
}
