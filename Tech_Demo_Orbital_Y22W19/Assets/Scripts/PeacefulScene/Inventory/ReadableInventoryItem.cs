using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReadableInventoryItem", menuName = "Assets/InventoryItems/ReadableInventoryItem")]
public class ReadableInventoryItem : InventoryItem
{
    [SerializeField]
    private CanvasGroup readableCanvas;

    public CanvasGroup ReadableCanvas => readableCanvas;
}
