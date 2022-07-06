using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public InventoryItem itemTest;
    public InventoryItem itemTest2;

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


    public void Start()
    {
        inventory.AddItem(itemTest);
        inventory.AddItem(itemTest2);
    }
}
