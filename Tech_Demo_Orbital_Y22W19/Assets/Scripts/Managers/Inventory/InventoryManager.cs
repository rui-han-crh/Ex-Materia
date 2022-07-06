using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager instance;

    public static InventoryManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InventoryManager>();
                Debug.Assert(instance != null, "There was no InventoryManager in this scene, consider adding one.");
            }
            return instance;
        }
    }

    private Inventory inventory;

    public InventoryItemsDatabase itemsDatabase;

    private DialogueRunner dialogueRunner;

    public IEnumerable<InventoryItem> InventoryItems => inventory.InventoryItems;

    private void OnEnable()
    {
        if (SaveFile.file.HasData(typeof(Inventory)))
        {
            inventory = SaveFile.file.Load<Inventory>(typeof(Inventory), "inventory");
        } 
        else
        {
            inventory = new Inventory();
        }
    }

    private void OnDisable()
    {
        SaveFile.file.Save(typeof(Inventory), "inventory", inventory);
    }

    private void Awake()
    {
        itemsDatabase = Instantiate(itemsDatabase, transform);
    }

    private void Start()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.AddCommandHandler("addItem", (string itemName) => AddItem(itemName));
        dialogueRunner.AddCommandHandler("removeItem", (string itemName) => RemoveItem(itemName));
    }

    public void AddItem(InventoryItem item)
    {
        inventory.AddItem(Instantiate(item));
    }

    public void RemoveItem(InventoryItem item)
    {
        inventory.RemoveItem(item);
    }

    public void AddItem(string itemName)
    {
        inventory.AddItem(itemsDatabase[itemName]);
    }

    public void RemoveItem(string itemName)
    {
        inventory.RemoveItem(itemsDatabase[itemName]);
    }
}
