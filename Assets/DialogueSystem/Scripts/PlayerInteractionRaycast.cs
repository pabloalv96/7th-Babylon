using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class PlayerInteractionRaycast : MonoBehaviour
{

    [SerializeField] private KeyCode selectInput = KeyCode.E;
    public KeyCode consumeInput = KeyCode.C;
    public KeyCode breakInput = KeyCode.F;

    [SerializeField] private float reachDistance = 5f;
    [SerializeField] private float selectionSize = 1f;

    public GameObject selectedObject;

    public GameObject interactPromptIndicator;
    public GameObject consumePromptIndicator;
    public GameObject breakPromptIndicator;
    public Image interactionAimIndicator;

    private bool isNPC;
    private bool isWorldDialogue;
    [HideInInspector] public bool isDoor;
    private bool isItem;
    private bool isConsumable;
    private bool isInteraction;
    private bool isLookSin;
    [HideInInspector] public bool isBreakable;

    [SerializeField] private GameObject lookSinObject;

    [SerializeField] private TextPopUp popUpText;
   


    private DoorActivator doorActivator;
    private OJQuestTrigger interaction;
    [SerializeField] private NPCInfo narrator;

    [SerializeField] private NPCDialogueOption interactDialogueOption;

    public LayerMask uiLayer;

    [SerializeField] private TextMeshProUGUI interactionPromptText;
    [SerializeField] private TextMeshProUGUI interactionKeyPromptText;
    [SerializeField] private TextMeshProUGUI consumePromptText;
    [SerializeField] private TextMeshProUGUI consumeKeyPromptText;
    [SerializeField] private TextMeshProUGUI breakPromptText;
    [SerializeField] private TextMeshProUGUI breakKeyPromptText;


    private DialogueListSystem dialogueSystem;
    private DialogueInitiator dialogueInitiator;
    private OJQuestManager questManager;
    private Inventory inventorySystem;
    //private PlayerDialogue playerDialogue;
    private PlayerInfoController playerInfoController;
    void Awake()
    {

        interactPromptIndicator.SetActive(false);
        consumePromptIndicator.SetActive(false);
        breakPromptIndicator.SetActive(false);

        dialogueSystem = FindObjectOfType<DialogueListSystem>();
        dialogueInitiator = FindObjectOfType<DialogueInitiator>();
        questManager = FindObjectOfType<OJQuestManager>();
        inventorySystem = FindObjectOfType<Inventory>();
        //playerDialogue = FindObjectOfType<PlayerDialogue>();
        playerInfoController = FindObjectOfType<PlayerInfoController>();

        //checkInventoryIndicator.text = "New Item! Press '" + selectInput + "' to check your inventory";

        //checkInventoryIndicator.enabled = false;
        //inventoryIndicatorDisplayTimeReset = inventoryIndicatorDisplayTime;

        consumeKeyPromptText.text = consumeInput.ToString();
        interactionKeyPromptText.text = selectInput.ToString();
        //breakKeyPromptText.text = consumeInput.ToString();
    }

    private void Update()
    {
        if (!dialogueSystem.enabled || (dialogueSystem.enabled && dialogueSystem.npcDialogue != null && dialogueSystem.npcDialogue.toOtherNPC))
        {
            StartCoroutine(InteractionRaycast());
        }
        //else
        //{ 

        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        dialogueSystem.responseTimer = 0f;


        //    }
        //}
    }

    public void SelectNPC()
    {
        if (selectedObject != null && selectedObject.GetComponent<NPCBrain>())
        {
            if (selectedObject.GetComponent<NPCBrain>().npcInfo != null)
            {
                selectedObject.GetComponent<NPCBrain>().SpeakingToPlayer();
                Debug.Log("NPC Selected: " + selectedObject.GetComponent<NPCBrain>().npcInfo.npcName);
                dialogueInitiator.EnterDialogue(selectedObject.GetComponent<NPCBrain>().npcInfo);
            }
        }
    }

    public void ActivateDialogueSystem()
    {
        if (selectedObject != null && selectedObject.GetComponent<DialogueInWorld>())
        {
            DialogueInWorld dialogueInWorld = selectedObject.GetComponent<DialogueInWorld>();
            Debug.Log("Dialogue Beginning: " + dialogueInWorld.dialogueFromWorld.name);

            dialogueInitiator.NPCInitiatedDialogue(dialogueInWorld.narrator, dialogueInWorld.dialogueFromWorld);
        }
    }

    public void PickUpItem()
    {
        InventoryItem item = selectedObject.GetComponent<ItemInWorld>().item;


        inventorySystem.AddItemToInventory(item);


        if (item.relatedQuests != null)
        {
            foreach (OJQuest quest in item.relatedQuests)
            {
                if (quest.questStarted && !quest.questEnded)
                {
                    if (quest.objective.objectiveType == OJQuestObjectiveType.itemBased)
                    {
                        foreach (OJQuestItemObjective questItem in quest.objective.questItems)
                        {
                            if (!questItem.questCompleted)
                            {
                                if (inventorySystem.CheckInventoryForItem(questItem.item) && inventorySystem.CheckItemCount(questItem.item) >= questItem.requiredAmount)
                                {
                                    questItem.requiredAmountCollected = true;
                                    if (quest.objective.questDialogueOptions.Count > 0)
                                    {
                                        questManager.AddQuestItemDialogue(quest.objective.questDialogueOptions[0], questItem);
                                    }
                                    else
                                    {
                                        questManager.EndQuest(quest);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        if (item.statsToEffectOnCollectionList.Count > 0)
        {
            playerInfoController.AffectStatValues(item.statsToEffectOnCollectionList);
        }


        popUpText.popUpText.text = item.itemName + " Collected! \n Press 'Q' to Check Your Inventory";

        popUpText.popUpIndicator = true;


        popUpText.DisplayPopUp();

        //if (audioSource.isPlaying)
        //{
        //    audioSource.Stop();

        //    audioSource.PlayOneShot(selectedObject.GetComponent<ItemInWorld>().itemCollectedAudio);
        //}

        //if (selectedObject.GetComponent<ItemInWorld>().hasDialogue)
        //{
        //    selectedObject.GetComponent<ItemInWorld>().unlockNewDialogue.enabled = true;
        //}

        //if (FindObjectOfType<PlayerInfoController>()) // progress collection quest if relevant
        //{
        //    PlayerInfoController player = FindObjectOfType<PlayerInfoController>();

        //    foreach (Quest quest in player.activeQuestList)
        //    {
        //        if (quest.questType == QuestType.Collection)
        //        {
        //            NumericalQuest numQuest = (NumericalQuest)quest;

        //            numQuest.SetCurrentAmount();
        //        }
        //    }
        //}
    }


    //public void ConsumeFood()
    //{
    //    if (selectedItem != null && selectedItem.canConsume)
    //    {
    //        playerInfoController.AffectStatValues(selectedItem.statsToEffectOnConsumptionList);
    //        playerInfoController.foodConsumed += 1;
    //        RemoveItemFromInventory(selectedItem);
    //    }
    //}
    public IEnumerator InteractionRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.SphereCast(ray, selectionSize, out hit, reachDistance, ~uiLayer)) //&& !initiateDialogue.dialogueSystem.enabled
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.magenta);

            if (hit.transform.GetComponent<ItemInWorld>())
            {
                isItem = true;
                selectedObject = hit.transform.gameObject;
                //Debug.Log("hit = " + selectedObject);
                interactPromptIndicator.SetActive(true);
                interactionAimIndicator.color = Color.red;
                interactionPromptText.text = "Collect " + selectedObject.GetComponent<ItemInWorld>().item.itemName;

                if (selectedObject.GetComponent<ItemInWorld>().item.canConsume)
                {
                    isConsumable = true;
                    consumePromptIndicator.SetActive(true);
                    consumePromptText.text = "Consume " + selectedObject.GetComponent<ItemInWorld>().item.itemName;
                }
                else
                {
                    isConsumable = false;
                    consumePromptIndicator.SetActive(false);

                }
            }
            else
            {
                isItem = false;
                isConsumable = false;
            }

            if (hit.transform.GetComponent<DialogueInWorld>())
            {
                selectedObject = hit.transform.gameObject;
                interactPromptIndicator.SetActive(true);
                interactionAimIndicator.color = Color.red;
                isWorldDialogue = true;
                interactionPromptText.text = "Inspect";

            }
            else
            {
                isWorldDialogue = false;
            }

            if (hit.transform.GetComponent<NPCBrain>())
            {
                isNPC = true;
                selectedObject = hit.transform.gameObject;
                //Debug.Log("hit = " + selectedObject);
                interactPromptIndicator.SetActive(true);
                interactionAimIndicator.color = Color.red;
                interactionPromptText.text = "Talk";

            }
            else
            {
                isNPC = false;
            }

            if (hit.transform.GetComponent<DoorActivator>())
            {
                isDoor = true;
                selectedObject = hit.transform.gameObject;
                interactPromptIndicator.SetActive(true);
                interactionAimIndicator.color = Color.red;

                if (selectedObject.GetComponent<DoorActivator>().isOpen)
                {
                    interactionPromptText.text = "Close Door";
                }
                else
                {
                    interactionPromptText.text = "Open Door";

                }

            }
            else
            {
                isDoor = false;
            }

            if (hit.transform.GetComponent<OJQuestTrigger>() && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                isInteraction = true;
                selectedObject = hit.transform.gameObject;
                interactPromptIndicator.SetActive(true);
                interactionAimIndicator.color = Color.red;
                interactionPromptText.text = "Inspect";

            }
            else
            {
                isInteraction = false;
            }

            if (hit.transform.GetComponent<LookSinTimer>())
            {
                isLookSin = true;
                lookSinObject = hit.transform.gameObject;
                Debug.Log("Hit Look Sin Object: " + lookSinObject.name);

            }
            else
            {
                isLookSin = false;
            }

            if (hit.transform.GetComponent<Breakable>())
            {
                isBreakable = true;
                selectedObject = hit.transform.gameObject;
                breakPromptIndicator.SetActive(true);
                interactionAimIndicator.color = Color.red;
                breakPromptText.text = "Break " + hit.transform.GetComponent<Breakable>().objectName;
            }
            else
            {
                isBreakable = false;
                breakPromptIndicator.SetActive(false);
            }


            if (selectedObject != null && isConsumable && Input.GetKeyDown(consumeInput))
            {
                playerInfoController.AffectStatValues(selectedObject.GetComponent<ItemInWorld>().item.statsToEffectOnConsumptionList);
                playerInfoController.foodConsumed += 1;
                Destroy(selectedObject.gameObject);
                //Play consume sound effect
            }

            if (selectedObject != null && Input.GetKeyDown(selectInput))
            {
                interactPromptIndicator.SetActive(false);
                //if (dialogueSystem.enabled)
                //{
                //    //dialogueSystem.responseTimer = 0f;
                //}

                if (isNPC)
                {
                    SelectNPC();
                    selectedObject.transform.GetComponent<NPCBrain>().isSpeakingToPlayer = true;
                }

                if (isWorldDialogue)
                {
                    ActivateDialogueSystem();
                    //Debug.Log("Hit dialogue in world game object");
                }

                if (isItem)
                {
                    PickUpItem();
                }

                //StartCoroutine(CheckInventoryIndicator());

                if (isDoor)
                {
                    doorActivator = selectedObject.GetComponent<DoorActivator>();
                    if (!doorActivator.isLocked)
                    {
                        if (doorActivator.isOpen)
                        {
                            doorActivator.CloseDoor();
                        }
                        else
                        {
                            doorActivator.OpenDoor();
                        }
                    }
                    else
                    {
                        //communicate locked door to player
                        //hint player about key

                        if (doorActivator.CheckKeysInInventory())
                        {

                            doorActivator.unlockDoorDialogue.playerResponses.Clear();

                            foreach (DoorKey key in doorActivator.keysList)
                            {
                                for (int i = 0; i < inventorySystem.inventory.Count; i++)
                                {
                                    if (inventorySystem.inventory[i] == key.keyItem)
                                    {
                                        PlayerDialogueOption newUnlockDialogue = new PlayerDialogueOption();

                                        newUnlockDialogue.isResponseToNPCDialogue = true;
                                        newUnlockDialogue.isGoodbyeOption = true;
                                        newUnlockDialogue.dialogue = "Unlock the door with " + key.keyItem.itemName;

                                        newUnlockDialogue.conditionalEvents = new List<UnityEvent>();

                                        newUnlockDialogue.conditionalEvents.Add(doorActivator.unlockDoorEvent);

                                        newUnlockDialogue.conditionalEvents.Add(doorActivator.openDoorEvent);

                                        newUnlockDialogue.statsToEffectList = key.statsToEffectList;

                                        doorActivator.unlockDoorDialogue.playerResponses.Add(newUnlockDialogue);



                                    }
                                }
                            }

                            dialogueInitiator.NPCInitiatedDialogue(narrator, doorActivator.unlockDoorDialogue);

                        }
                        else
                        {
                            dialogueInitiator.NPCInitiatedDialogue(narrator, doorActivator.lockedDoorDialogue);

                        }

                    }
                }

                if (isInteraction)
                {
                    interaction = selectedObject.GetComponent<OJQuestTrigger>();

                    interactDialogueOption.playerResponses.Clear();

                    foreach (EnvironmentalItemInteraction itemInteraction in interaction.itemInteractionsList)
                    {
                        if (inventorySystem.CheckInventoryForItem(itemInteraction.item))
                        {

                            PlayerDialogueOption interactionDialogue = new PlayerDialogueOption();

                            interactionDialogue.isResponseToNPCDialogue = true;
                            interactionDialogue.isGoodbyeOption = true;
                            interactionDialogue.dialogue = "Use " + itemInteraction.item.itemName + " on " + interaction.interactionObjectName;

                            interactionDialogue.conditionalEvents = new List<UnityEvent>();

                            foreach (UnityEvent itemEvent in itemInteraction.itemInteractionEvents)
                            {
                                interactionDialogue.conditionalEvents.Add(itemEvent);
                            }

                            interactionDialogue.statsToEffectList = itemInteraction.statsToEffectList;

                            interactDialogueOption.playerResponses.Add(interactionDialogue);

                        }
                    }

                    dialogueInitiator.NPCInitiatedDialogue(narrator, interactDialogueOption);

                }
            }
            else if (selectedObject != null && Input.GetKeyDown(breakInput))
            {
                if (isBreakable)
                {
                    Debug.Log(selectedObject.name + " has been broken");

                    selectedObject.GetComponent<Breakable>().BreakObject();



                }
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            //Debug.Log("Did not Hit");
            selectedObject = null;
            interactPromptIndicator.SetActive(false);
            consumePromptIndicator.SetActive(false);
            breakPromptIndicator.SetActive(false);

            interactionAimIndicator.color = Color.white;
        }

        if (lookSinObject != null)
        {
            if (isLookSin)
            {
                lookSinObject.GetComponent<LookSinTimer>().isLooking = true;
            }
            else
            {
                isLookSinInteracted = false;
                lookSinObject.GetComponent<LookSinTimer>().isLooking = false;
            }
        }
        

        yield return null;
    }

   
}



//void Update()
//{
//    Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
//    RaycastHit hit;

//    if (Physics.SphereCast(ray, selectionSize, out hit, reachDistance, ~uiLayer)) //&& !initiateDialogue.dialogueSystem.enabled
//    {
//        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.magenta);

//        if (hit.transform.GetComponent<ItemInWorld>())
//        {
//            isItem = true;
//            selectedObject = hit.transform.gameObject;
//            Debug.Log("hit = " + selectedObject);
//            interactPromptIndicator.SetActive(true);
//            interactionAimIndicator.color = Color.red;
//        }
//        else
//        {
//            isItem = false;
//        }

//        if (hit.transform.GetComponent<DialogueInWorld>())
//        {
//            selectedObject = hit.transform.gameObject;
//            interactPromptIndicator.SetActive(true);
//            interactionAimIndicator.color = Color.red;
//            isWorldDialogue = true;
//        }
//        else
//        {
//            isWorldDialogue = false;
//        }

//        if (hit.transform.GetComponent<NPCBrain>())
//        {
//            isNPC = true;
//            selectedObject = hit.transform.gameObject;
//            //Debug.Log("hit = " + selectedObject);
//            interactPromptIndicator.SetActive(true);
//            interactionAimIndicator.color = Color.red;

//        }
//        else
//        {
//            isNPC = false;
//        }

//        if (hit.transform.GetComponent<DoorActivator>())
//        {
//            isDoor = true;
//            selectedObject = hit.transform.gameObject;
//            interactPromptIndicator.SetActive(true);
//            interactionAimIndicator.color = Color.red;
//        }
//        else
//        {
//            isDoor = false;
//        }

//        if (hit.transform.GetComponent<OJQuestInteraction>())
//        {
//            isInteraction = true;
//            selectedObject = hit.transform.gameObject;
//            interactPromptIndicator.SetActive(true);
//            interactionAimIndicator.color = Color.red;
//        }
//        else
//        {
//            isInteraction = false;
//        }

//        if (selectedObject != null && Input.GetKeyDown(selectInput))
//        {
//            if (initiateDialogue.dialogueSystem.enabled)
//            {
//                initiateDialogue.dialogueSystem.responseTimer = 0f;
//            }

//            if (isNPC)
//            {
//                SelectNPC();
//                selectedObject.transform.GetComponent<NPCBrain>().isSpeakingToPlayer = true;
//            }

//            if (isWorldDialogue)
//            {
//                ActivateDialogueSystem();
//                Debug.Log("Hit dialogue in world game object");
//            }

//            if (isItem)
//            {
//                PickUpItem();
//            }

//            //StartCoroutine(CheckInventoryIndicator());

//            if (isDoor)
//            {
//                doorActivator = selectedObject.GetComponent<DoorActivator>();
//                if (!doorActivator.isLocked)
//                {
//                    if (doorActivator.isOpen)
//                    {
//                        doorActivator.CloseDoor();
//                    }
//                    else
//                    {
//                        doorActivator.OpenDoor();
//                    }
//                }
//                else
//                {
//                    //communicate locked door to player
//                    //hint player about key

//                    if(doorActivator.CheckKeysInInventory())
//                    {

//                        unlockDoorDialogue.playerResponses.Clear();

//                        foreach(DoorKey key in doorActivator.keysList)
//                        {
//                            for (int i = 0; i < inventory.inventory.Count; i++)
//                            {
//                                if (inventory.inventory[i] == key.keyItem)
//                                {
//                                    PlayerDialogueOption newUnlockDialogue = new PlayerDialogueOption();

//                                    newUnlockDialogue.isResponseToNPCDialogue = true;
//                                    newUnlockDialogue.isGoodbyeOption = true;
//                                    newUnlockDialogue.dialogue = "Unlock the door with " + key.keyItem.itemName;

//                                    newUnlockDialogue.conditionalEvents = new List<UnityEvent>();

//                                    newUnlockDialogue.conditionalEvents.Add(doorActivator.unlockDoorEvent);

//                                    newUnlockDialogue.conditionalEvents.Add(doorActivator.openDoorEvent);

//                                    newUnlockDialogue.statsToEffectList = key.statsToEffectList;

//                                    unlockDoorDialogue.playerResponses.Add(newUnlockDialogue);



//                                }
//                            }
//                        }

//                        FindObjectOfType<StartDialogue>().NPCInitiatedDialogue(narrator, unlockDoorDialogue);

//                    }
//                    else
//                    {
//                        FindObjectOfType<StartDialogue>().NPCInitiatedDialogue(narrator, lockedDoorDialogue);

//                    }

//                }
//            }

//            if (isInteraction)
//            {
//                interaction = selectedObject.GetComponent<OJQuestInteraction>();

//                if (interaction.CheckItemsnInventory())
//                {
//                    interactDialogueOption.playerResponses.Clear();

//                    foreach (EnvironmentalItemInteraction itemInteraction in interaction.itemInteractionsList)
//                    {
//                        for (int i = 0; i < inventory.inventory.Count; i++)
//                        {
//                            if (inventory.inventory[i] == itemInteraction.item)
//                            {
//                                PlayerDialogueOption interactionDialogue = new PlayerDialogueOption();

//                                interactionDialogue.isResponseToNPCDialogue = true;
//                                interactionDialogue.isGoodbyeOption = true;
//                                interactionDialogue.dialogue = "Use " + itemInteraction.item.itemName + " on " + interaction.interactionObjectName;

//                                interactionDialogue.conditionalEvents = new List<UnityEvent>();

//                                foreach (UnityEvent itemEvent in itemInteraction.itemInteractionEvents)
//                                {
//                                    interactionDialogue.conditionalEvents.Add(itemEvent);
//                                }

//                                interactionDialogue.statsToEffectList = itemInteraction.statsToEffectList;

//                                interactDialogueOption.playerResponses.Add(interactionDialogue);
//                            }
//                        }
//                    }

//                    FindObjectOfType<StartDialogue>().NPCInitiatedDialogue(narrator, interactDialogueOption);

//                }
//            }
//        }
//    }
//    else
//    {
//        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
//        //Debug.Log("Did not Hit");
//        selectedObject = null;
//        interactPromptIndicator.SetActive(false);
//        interactionAimIndicator.color = Color.white;
//    }


//}
