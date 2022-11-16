using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Inventory : MonoBehaviour
{
    //public UnityEvent onInventoryNumInput;

    //List of Inventory UI Elements (different element per new item, duplicate items stack)
    [SerializeField] private List<TextMeshProUGUI> inventoryListUI;

    //List of what's in your inventory (duplicate items are added as an increase to the "numCarried" variable in the respective 'InventoryItem' scriptable object)
    public List<InventoryItem> inventory;
    public int inventorySlotLimit;
    

    //Prefab of the Inventory UI Element
    [SerializeField] private GameObject inventoryItemUIPrefab;

    //Inventory UI Panel
    [SerializeField] private GameObject inventoryPanel;

    private Transform player;

    [SerializeField] private NPCInfo narrator;
    [SerializeField] private NPCDialogueOption itemCapacityReachedDialogue; // if the player has reached the max limit for a specific item
    [SerializeField] private NPCDialogueOption slotLimitReachedDialogue; // if the player has filled all their slots

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void AddItemToInventory(InventoryItem item)
    {
        //if item is already in inventory increase num carried (in 'InventoryItem' scriptable object)
        if (inventory.Contains(item))
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] == item)
                {
                    if (item.numCarried < item.maxNumCarried)
                    {
                        inventory[i].numCarried += 1;
                        inventoryListUI[i].text = item.itemName + " x " + item.numCarried;
                    }
                    else
                    {
                        //display a dialogue panel if the player can't pick up an item
                        FindObjectOfType<StartDialogue>().NPCInitiatedDialogue(narrator, itemCapacityReachedDialogue);
                    }
                }
            }
        }
        else // otherwise add a new item to the inventory
        {
            if (inventory.Count - 1 < inventorySlotLimit)
            {
                inventory.Add(item);
                item.numCarried = 1;
                GameObject newItemUI = Instantiate(inventoryItemUIPrefab, inventoryPanel.transform);
                TextMeshProUGUI newItemText = newItemUI.GetComponent<TextMeshProUGUI>();
                newItemText.text = item.itemName + " x " + item.numCarried;
                TextMeshProUGUI newItemNumText = newItemUI.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                newItemNumText.text = inventory.Count + 1.ToString();
                inventoryListUI.Add(newItemText);
            }
            else
            {
                //display a dialogue panel if the player can't pick up an item
                FindObjectOfType<StartDialogue>().NPCInitiatedDialogue(narrator, slotLimitReachedDialogue);
            }
        }
    }

    // Drop and Remove have not been tested
    public void DropItem(InventoryItem item)
    {
        RemoveItemFromInventory(item);
        GameObject droppedItem = Instantiate(item.prefab, player);
        droppedItem.transform.parent = null;
    }

    public void RemoveItemFromInventory(InventoryItem item)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == item)
            {
                inventory[i].numCarried -= 1;

                if (inventory[i].numCarried <= 0f)
                {
                    inventory.RemoveAt(i);
                    Destroy(inventoryListUI[i].gameObject);
                }
            }
        }
    }



}
