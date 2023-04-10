using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour/*, IDragHandler*/
{
    public InventoryItem selectedItem;

    // inventory inputs
    [SerializeField] private KeyCode dropItemInput = KeyCode.Z;
    //[SerializeField] private KeyCode inspectItemInput = KeyCode.V;
    [SerializeField] private KeyCode cancelSelectionInput = KeyCode.X;
    [SerializeField] private KeyCode consumeItemInput = KeyCode.C;

    [SerializeField] private GameObject dropItemUIPrompt;
    [SerializeField] private GameObject cancelSelectionUIPrompt;
    [SerializeField] private GameObject consumeItemUIPrompt;

    //inspect item variables
    //[SerializeField] private GameObject inspectItemUIPrompt;
    //[SerializeField] private GameObject inspectItemUIControls;
    //public bool isInspectingItem;

    [SerializeField] private GameObject inspectedItem;

    [SerializeField] private Transform inspectBasePos;
    [SerializeField] private float inspectRotationSpeed;
    //[SerializeField] private Vector3 inspectInitalPos;
    //[SerializeField] private Quaternion inspectedItemRotation;

    //[SerializeField] private float inspectFarZoomDistance;
    //[SerializeField] private float inspectCloseZoomDistance;
    //[SerializeField] private float scrollSpeedScale;
    //UnityEvent onInspectionZoom;


    [SerializeField] private Color selectedColour;

    //List of Inventory UI Elements (different element per new item, duplicate items stack)
    [SerializeField] private List<TextMeshProUGUI> inventoryListUI;

    
    //List of what's in your inventory (duplicate items are added as an increase to the "numCarried" variable in the respective 'InventoryItem' scriptable object)
    public List<InventoryItem> inventory;
    public int inventorySlotLimit;
    

    //Prefab of the Inventory UI Element
    [SerializeField] private GameObject inventoryItemUIPrefab;

    //Inventory UI Panel
    public GameObject inventoryPanel;

    private Transform player;

    [SerializeField] private NPCInfo narrator;
    [SerializeField] private NPCDialogueOption itemCapacityReachedDialogue; // if the player has reached the max limit for a specific item
    [SerializeField] private NPCDialogueOption slotLimitReachedDialogue; // if the player has filled all their slots


    private DialogueListSystem dialogueSystem;
    private DialogueInitiator dialogueInitiator;
    private OJQuestManager questManager;
    //private Inventory inventorySystem;
    //private PlayerDialogue playerDialogue;
    private PlayerInfoController playerInfoController;
    private PlayerInteractionRaycast playerInteractionRaycast;


    private void Awake()
    {
        dialogueSystem = FindObjectOfType<DialogueListSystem>();
        dialogueInitiator = FindObjectOfType<DialogueInitiator>();
        questManager = FindObjectOfType<OJQuestManager>();
        //inventorySystem = FindObjectOfType<Inventory>();
        //playerDialogue = FindObjectOfType<PlayerDialogue>();
        playerInfoController = FindObjectOfType<PlayerInfoController>();
        playerInteractionRaycast = FindObjectOfType<PlayerInteractionRaycast>();
        playerInteractionRaycast = FindObjectOfType<PlayerInteractionRaycast>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        dropItemUIPrompt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = dropItemInput.ToString();
        cancelSelectionUIPrompt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cancelSelectionInput.ToString();
        consumeItemUIPrompt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = consumeItemInput.ToString();
        //inspectItemUIPrompt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = inspectItemInput.ToString();

        //if (onInspectionZoom == null)
        //{
        //    onInspectionZoom = new UnityEvent();
        //}

        //onInspectionZoom.AddListener(InspectionZoom);
    }

    private void Update()
    {
        SelectInventoryItem();

        if (inventoryPanel.activeSelf)
        {
            if (selectedItem != null)
            {
                SetSelectedItemTextColour();
                InspectItem();

                //enable drop object ui prompt
                //enable inspect UI prompt
                //Narrator text when inspecting?

                dropItemUIPrompt.SetActive(true);
                //inspectItemUIPrompt.SetActive(true);
                cancelSelectionUIPrompt.SetActive(true);

                if (selectedItem.canConsume)
                {
                    consumeItemUIPrompt.SetActive(true);
                    if (Input.GetKeyDown(consumeItemInput))
                    {
                        ConsumeFood();
                    }
                }

                if (Input.GetKeyDown(dropItemInput))
                {

                    DropItem(selectedItem);

                }

                if (Input.GetKeyDown(cancelSelectionInput))
                {
                    selectedItem = null;
                    SetSelectedItemTextColour();
                }
            }
            else
            {
                dropItemUIPrompt.SetActive(false);
                //inspectItemUIPrompt.SetActive(false);
                cancelSelectionUIPrompt.SetActive(false);
                consumeItemUIPrompt.SetActive(false);

                selectedItem = null;
                SetSelectedItemTextColour();
                if (inspectedItem != null)
                {
                    EndItemInspection();
                }
            }
        }
        else
        {
            dropItemUIPrompt.SetActive(false);
            //inspectItemUIPrompt.SetActive(false);
            cancelSelectionUIPrompt.SetActive(false);
            consumeItemUIPrompt.SetActive(false);
            selectedItem = null;
            SetSelectedItemTextColour();
            if (inspectedItem != null)
            {
                EndItemInspection();
            }
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

                        if (playerInteractionRaycast.selectedObject.GetComponent<ItemInWorld>())
                        {
                            Destroy(playerInteractionRaycast.selectedObject);
                            playerInteractionRaycast.selectedObject = null;
                            playerInteractionRaycast.interactPromptIndicator.SetActive(false);
                            playerInteractionRaycast.consumePromptIndicator.SetActive(false);

                        }
                    }
                    else
                    {
                        //display a dialogue panel if the player can't pick up an item
                        dialogueInitiator.NPCInitiatedDialogue(narrator, itemCapacityReachedDialogue);
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

                if (playerInteractionRaycast.selectedObject.GetComponent<ItemInWorld>())
                {
                    Destroy(playerInteractionRaycast.selectedObject);
                    playerInteractionRaycast.selectedObject = null;
                    playerInteractionRaycast.interactPromptIndicator.SetActive(false);
                    playerInteractionRaycast.consumePromptIndicator.SetActive(false);

                }
            }
            else
            {
                //display a dialogue panel if the player can't pick up an item
                dialogueInitiator.NPCInitiatedDialogue(narrator, slotLimitReachedDialogue);
            }
        }
    }

    // Drop and Remove have not been tested
    public void DropItem(InventoryItem item)
    {
        Quaternion inspectedItemRotation = inspectedItem.transform.rotation;
            RemoveItemFromInventory(item);

            GameObject droppedItem = Instantiate(item.prefab, inspectBasePos.position, inspectedItemRotation);
            droppedItem.transform.parent = null;

        if (droppedItem.GetComponent<Breakable>())
        {
            droppedItem.GetComponent<Breakable>().BreakObject();
        }


            foreach (OJQuest quest in questManager.activeQuestList)
        {
            if (quest.objective.objectiveType == OJQuestObjectiveType.itemBased)
            {
                foreach (OJQuestItemObjective questItemObjective in quest.objective.questItems)
                {
                    if (!CheckInventoryForItem(questItemObjective.item) || CheckItemCount(questItemObjective.item) < questItemObjective.requiredAmount)
                    {
                        questItemObjective.requiredAmountCollected = false;
                        Debug.Log("Quest Requirement No Longer Reached. Dialogue Has been Removed");
                    }

                }
            }
        }
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
                    EndItemInspection();
                    SetSelectedItemTextColour();
                }
            }
        }
    }

    public bool CheckInventoryForItem(InventoryItem desiredItem)
    {
        // create dialogue option for each applicable key in inventory
        foreach (InventoryItem item in inventory)
        {
                if (item == desiredItem)
                {
                    return true;
                }
            
        }

        return false;
    }

    public int CheckItemCount(InventoryItem desiredItem)
    {
        foreach(InventoryItem item in inventory)
        {
            if (item == desiredItem)
            {
                return item.numCarried;
            }

        }

        return 0;
    }

    public bool CheckIfConsumableItem(InventoryItem desiredItem)
    {
        foreach (InventoryItem item in inventory)
        {
            if (item == desiredItem)
            {
                return item.canConsume;
            }

        }

        return false;
    }

    private void InspectItem()
    {
        if (selectedItem != null)
        {
            //Cursor.lockState = CursorLockMode.Confined;
            //Cursor.visible = true;

            //FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;

            //if (Input.GetMouseButtonDown(0))
            //{
            //    FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
            //}
            //else if (!dialogueSystem.inDialogue)
            //{
            //    FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
            //}


            if (inspectedItem == null)
            {
                inspectedItem = Instantiate(selectedItem.prefab, inspectBasePos.position, Quaternion.identity);
                inspectedItem.transform.parent = inspectBasePos;

                inspectedItem.GetComponent<ItemInWorld>().enabled = false;
                inspectedItem.GetComponent<Collider>().enabled = false;
                //inspectInitalPos = inspectedItem.transform.position;

                //inspectedItemRotation = Quaternion.Euler(inspectedItem.transform.localEulerAngles);

                if (inspectedItem.GetComponent<Rigidbody>() && inspectedItem.GetComponent<Rigidbody>().useGravity != false)
                {
                    inspectedItem.GetComponent<Rigidbody>().useGravity = false;
                }

                //isInspectingItem = true;
            }
            //else if (selectedItem != inspectedItem)
            //{
            //    EndItemInspection();

            //    InspectItem();
            //}

            //if (Input.mouseScrollDelta.y != 0)
            //{
            //    onInspectionZoom.Invoke();
            //}

            if (Input.GetKeyDown(cancelSelectionInput))
            {
                EndItemInspection();
            }
        }
    }

    private void EndItemInspection()
    {
        if (!dialogueSystem.inDialogue)
        {
            FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        Destroy(inspectedItem);

        inspectedItem = null;
        //isInspectingItem = false;
        //selectedItem = null;
        //SetSelectedItemTextColour();


    }

    //private void InspectionZoom()
    //{
    //    if (Vector3.Distance(inspectedItem.transform.position, player.transform.position) < inspectCloseZoomDistance) // move towards player
    //    {
    //        //if (Input.mouseScrollDelta.y > 0)
    //        //{
    //        //    inspectedItem.transform.position = Vector3.MoveTowards(inspectedItem.transform.position, -Camera.main.transform.forward, inspectCloseZoomDistance);
    //        inspectedItem.transform.position += player.transform.forward * Input.mouseScrollDelta.y * scrollSpeedScale * Time.deltaTime;

    //        //}
    //    }

    //    if (Vector3.Distance(inspectedItem.transform.position, player.transform.position) > inspectFarZoomDistance) // move away from player
    //    {
    //        inspectedItem.transform.position += player.transform.forward * Input.mouseScrollDelta.y * scrollSpeedScale * Time.deltaTime;
    //    }
    //}
    //private void InspectionRotate()
    //{
    //    float horizontal = Input.GetAxisRaw("Horizontal");
    //    float vertical = Input.GetAxisRaw("Vertical");

    //    Vector2 input = new Vector2(horizontal, vertical);

    //    // normalize input if it exceeds 1 in combined length:
    //    if (input.sqrMagnitude > 1)
    //    {
    //        input.Normalize();
    //    }

    //}

    public void ConsumeFood()
    {
        if (selectedItem != null && selectedItem.canConsume)
        {
            playerInfoController.AffectStatValues(selectedItem.statsToEffectOnConsumptionList);
            playerInfoController.foodConsumed += 1;
            RemoveItemFromInventory(selectedItem);
        }
    }

    private void SelectInventoryItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (inventory.Count >= 1 && inventorySlotLimit >= 1)
            {
                selectedItem = inventory[0];
                EndItemInspection();
                InspectItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (inventory.Count >= 2 && inventorySlotLimit >= 2)
            {
                selectedItem = inventory[1];
                EndItemInspection();
                InspectItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (inventory.Count >= 3 && inventorySlotLimit >= 3)
            {
                selectedItem = inventory[2];
                EndItemInspection();
                InspectItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (inventory.Count >= 4 && inventorySlotLimit >= 4)
            {
                selectedItem = inventory[3];
                EndItemInspection();
                InspectItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (inventory.Count >= 5 && inventorySlotLimit >= 5)
            {
                selectedItem = inventory[4];
                EndItemInspection();
                InspectItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (inventory.Count >= 6 && inventorySlotLimit >= 6)
            {
                selectedItem = inventory[5];
                EndItemInspection();
                InspectItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (inventory.Count >= 7 && inventorySlotLimit >= 7)
            {
                selectedItem = inventory[6];
                EndItemInspection();
                InspectItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            if (inventory.Count >= 8 && inventorySlotLimit >= 8)
            {
                selectedItem = inventory[7];
                EndItemInspection();
                InspectItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (inventory.Count >= 9 && inventorySlotLimit >= 9)
            {
                selectedItem = inventory[8];
                EndItemInspection();
                InspectItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (inventory.Count >= 10 && inventorySlotLimit >= 10)
            {
                selectedItem = inventory[9];
                EndItemInspection();
                InspectItem();
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

    //public void OnDrag(PointerEventData eventData)
    //{
    //    if (inventoryPanel.activeSelf)
    //    {
    //        Debug.Log("OnDrag");

    //        inspectedItem.transform.eulerAngles += new Vector3(eventData.delta.y , eventData.delta.x );
    //    }
    //}
}
