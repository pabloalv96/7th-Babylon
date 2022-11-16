using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;

public class Inventory : MonoBehaviour
{
    //public UnityEvent onInventoryNumInput;

    public InventoryItem selectedItem;

    [SerializeField] private KeyCode dropItemInput;
    //[SerializeField] private KeyCode inspectItemInput;
    [SerializeField] private KeyCode cancelSelectionInput;

    [SerializeField] private GameObject dropItemUI;
    //[SerializeField] private GameObject inspectItemUI;
    [SerializeField] private GameObject cancelSelectionUI;


    [SerializeField] private Color selectedColour;

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

        dropItemUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = dropItemInput.ToString();
        cancelSelectionUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cancelSelectionInput.ToString();

        //if (onInventoryNumInput == null)
        //{
        //    onInventoryNumInput = new UnityEvent();
        //}

        //onInventoryNumInput.AddListener(SelectInventoryItem);
    }

    private void Update()
    {
        SelectInventoryItem();

        if (selectedItem != null)
        {
            SetSelectedItemTextColour();

            //enable drop object ui prompt
            //enable inspect UI prompt
            //Narrator text when inspecting?

            dropItemUI.SetActive(true);
            //inspectItemUI.SetActive(true);
            cancelSelectionUI.SetActive(true);

            if (Input.GetKeyDown(dropItemInput))
            {

                DropItem(selectedItem);
                
            }

            //if (Input.GetKeyDown(inspectItemInput))
            //{
            ////activate inspect item view
            //}

            if (Input.GetKeyDown(cancelSelectionInput))
            {
                selectedItem = null;
                SetSelectedItemTextColour();
            }
        }
        else
        {
            dropItemUI.SetActive(false);
            //inspectItemUI.SetActive(false);
            cancelSelectionUI.SetActive(false);

            

        }
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

                        Destroy(FindObjectOfType<PlayerInteractionRaycast>().selectedObject);
                        FindObjectOfType<PlayerInteractionRaycast>().selectedObject = null;
                        FindObjectOfType<PlayerInteractionRaycast>().interactIndicator.SetActive(false);
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
            if (inventory.Count < inventorySlotLimit)
            {
                inventory.Add(item);
                item.numCarried = 1;
                GameObject newItemUI = Instantiate(inventoryItemUIPrefab, inventoryPanel.transform);
                TextMeshProUGUI newItemText = newItemUI.GetComponent<TextMeshProUGUI>();
                newItemText.text = item.itemName + " x " + item.numCarried;
                TextMeshProUGUI newItemNumText = newItemUI.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                newItemNumText.text = inventory.Count.ToString();
                inventoryListUI.Add(newItemText);

                Destroy(FindObjectOfType<PlayerInteractionRaycast>().selectedObject);
                FindObjectOfType<PlayerInteractionRaycast>().selectedObject = null;
                FindObjectOfType<PlayerInteractionRaycast>().interactIndicator.SetActive(false);
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
        GameObject droppedItem = Instantiate(item.prefab, player.position + new Vector3(0.0f, 1f, 0.5f), Quaternion.identity);
        droppedItem.transform.parent = null;
    }

    public void RemoveItemFromInventory(InventoryItem item)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == item)
            {
                inventory[i].numCarried--;
                inventoryListUI[i].text = item.itemName + " x " + item.numCarried;

                if (inventory[i].numCarried <= 0f)
                {
                    Destroy(inventoryListUI[i].gameObject);

                    inventory.RemoveAt(i);
                    inventoryListUI.RemoveAt(i);

                    for (int x = 0; x < inventoryListUI.Count; x++)
                    {
                        TextMeshProUGUI newItemNumText = inventoryListUI[x].transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                        newItemNumText.text = (x + 1).ToString();
                    }

                    selectedItem = null;
                    SetSelectedItemTextColour();
                }
            }
        }
    }

    private void SelectInventoryItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (inventory.Count >= 1 && inventorySlotLimit >= 1)
            {
                selectedItem = inventory[0];
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (inventory.Count >= 2 && inventorySlotLimit >= 2)
            {
                selectedItem = inventory[1];
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (inventory.Count >= 3 && inventorySlotLimit >= 3)
            {
                selectedItem = inventory[2];
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (inventory.Count >= 4 && inventorySlotLimit >= 4)
            {
                selectedItem = inventory[3];
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (inventory.Count >= 5 && inventorySlotLimit >= 5)
            {
                selectedItem = inventory[4];
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (inventory.Count >= 6 && inventorySlotLimit >= 6)
            {
                selectedItem = inventory[5];
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (inventory.Count >= 7 && inventorySlotLimit >= 7)
            {
                selectedItem = inventory[6];
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            if (inventory.Count >= 8 && inventorySlotLimit >= 8)
            {
                selectedItem = inventory[7];
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (inventory.Count >= 9 && inventorySlotLimit >= 9)
            {
                selectedItem = inventory[8];
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (inventory.Count >= 10 && inventorySlotLimit >= 10)
            {
                selectedItem = inventory[9];
            }
        }
    }

    private void SetSelectedItemTextColour()
    {
        foreach (TextMeshProUGUI itemUI in inventoryListUI)
        {
            if (selectedItem != null)
            {
                if (itemUI.text.Contains(selectedItem.itemName) && itemUI.text.Contains(selectedItem.numCarried.ToString()))
                {
                    itemUI.color = selectedColour;
                }
                else
                {
                    itemUI.color = Color.white;
                }
            }
            else
            {
                itemUI.color = Color.white;
            }
        }
    }

}
