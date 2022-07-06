using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "Assets/InventoryItems/InventoryItem")]
public class InventoryItem : ScriptableObject
{
    [SerializeField]
    private Sprite sprite;

    public Sprite Sprite => sprite;

    [SerializeField]
    private string itemName;

    public string ItemName => itemName;

    [SerializeField]
    private string description;

    public string Description => description;

    public override int GetHashCode()
    {
        return itemName.GetHashCode() ^ description.GetHashCode();
    }

    public override bool Equals(object other)
    {
        if (!(other is InventoryItem))
        {
            return false;
        }
        InventoryItem otherItem = (InventoryItem)other;
        return otherItem.itemName == itemName && otherItem.description == description;
    }
}
