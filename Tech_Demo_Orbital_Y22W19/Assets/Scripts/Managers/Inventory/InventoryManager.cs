using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

public class InventoryManager : MonoBehaviour, ISaveable
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


    private void Awake()
    {
        itemsDatabase = Instantiate(itemsDatabase, transform);
        inventory = new Inventory();
    }

    private void Start()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.AddCommandHandler("addItem", (string itemName) => AddItem(itemName));
        dialogueRunner.AddCommandHandler("removeItem", (string itemName) => RemoveItem(itemName));
    }

    public void LoadData()
    {
        inventory = new Inventory();

        if (SaveFile.file.HasData(typeof(Inventory)))
        {
            string[] itemNames = SaveFile.file.Load<string[]>(typeof(Inventory), "inventoryItems");

            foreach (string itemName in itemNames)
            {
                AddItem(itemName);
            }

            Debug.Log($"Loading {itemNames.Length} items");
        }
    }

    public void SaveData()
    {
        SaveFile.file.Save(typeof(Inventory), "inventoryItems", inventory.ItemsToString());
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
