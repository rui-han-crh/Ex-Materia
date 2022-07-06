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
}
