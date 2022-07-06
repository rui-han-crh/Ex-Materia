using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Inventory
{
    private HashSet<InventoryItem> inventoryItems;

    public IEnumerable<InventoryItem> InventoryItems => inventoryItems.ToArray();

    public Inventory()
    {
        inventoryItems = new HashSet<InventoryItem>();
    }

    public Inventory(IEnumerable<InventoryItem> items)
    {
        inventoryItems = new HashSet<InventoryItem>(items);
    }

    public void AddItem(InventoryItem item)
    {
        inventoryItems.Add(item);
    }

    public void RemoveItem(InventoryItem item)
    {
        inventoryItems.Remove(item);
    }
}
