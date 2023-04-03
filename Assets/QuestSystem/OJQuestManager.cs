using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OJQuestManager : MonoBehaviour
{
    public TextMeshProUGUI questUIPrefab;

    public List<TextMeshProUGUI> activeQuestUiList;

    public GameObject activeQuestUIParent;

    public List<OJQuest> activeQuestList;

    public List<OJQuest> completedQuestList;
    public List<OJQuest> missedQuestList;

    public NPCInfo narrator;

    public List<Collider> questColliders;

    //public List<InvisibleBarrier> invisibleBarriers;

    private DialogueListSystem dialogueSystem;
    //private DialogueInitiator dialogueInitiator;
    //private OJQuestManager questManager;
    private Inventory inventorySystem;
    private PlayerDialogue playerDialogue;
    private PlayerInfoController playerInfoController;

    [SerializeField] private TextPopUp popUpText;

    public List<OJQuest> listOfAllQuests;
    public List<PlayerDialogueOption> lockedAtStartDialogue;
    public List<PlayerDialogueOption> unlockedAtStartDialogue;



    // track currently active quests
    // lock quests that are completed or no longer available

    //[System.Serializable]
    //public class InvisibleBarrier
    //{
    //    public GameObject barrierObject;
    //    public string barrierID;
    //}
    private void Awake()
    {
        questColliders = new List<Collider>();

        foreach (OJQuestTrigger questInScene in FindObjectsOfType<OJQuestTrigger>())
        {
            if (questInScene.GetComponent<Collider>() && questInScene.GetComponent<Collider>().isTrigger)
            {
                questColliders.Add(questInScene.GetComponent<Collider>());
            }
        }

        dialogueSystem = FindObjectOfType<DialogueListSystem>();
        //dialogueInitiator = FindObjectOfType<DialogueInitiator>();
        //questManager = FindObjectOfType<OJQuestManager>();
        inventorySystem = FindObjectOfType<Inventory>();
        playerDialogue = FindObjectOfType<PlayerDialogue>();
        playerInfoController = FindObjectOfType<PlayerInfoController>();
    }

    public void StartQuest(OJQuest quest)
    {
        // add quest to active quest list
        if (((!quest.questStarted && !quest.questEnded) || quest.questEnded && quest.isRepeatable) && !activeQuestList.Contains(quest))
        {

            quest.questStarted = false;
            quest.questEnded = false;

            popUpText.popUpText.text = "New Task Recieved. Press Q to Check";

            popUpText.popUpIndicator = true;


            popUpText.DisplayPopUp();


            if (!quest.isHiddenFromUI)
            {
                TextMeshProUGUI questUIText = Instantiate(questUIPrefab, activeQuestUIParent.transform);
                questUIText.text = quest.questDescription;

                activeQuestUiList.Add(questUIText);
            }

            activeQuestList.Add(quest);

            ActivateQuestInteractions(quest);

            foreach (OJQuest questToLock in quest.questActivation.questsToLock)
            {
                EndQuest(questToLock);
            }

            //if (quest.objective.objectiveType == OJQuestObjectiveType.dialogueBased && quest.objective.isItemDialogue)
            //{
            //    foreach (OJQuestItemObjective itemObjective in quest.objective.questItems)
            //    {
            //        if (inventorySystem.CheckInventoryForItem(itemObjective.item) && inventorySystem.CheckItemCount(itemObjective.item) >= itemObjective.requiredAmount)
            //        {
            //            foreach (OJQuest itemQuest in itemObjective.item.relatedQuests)
            //            {
            //                EndQuest(itemQuest);
            //            }
            //        }
            //    }
            //}

            quest.questStarted = true;
        }

        // create quest UI
    }

    public void RemoveInactiveQuestUI()
    {

        for (int i = 0; i < activeQuestUiList.Count; i++)
        {
            foreach (OJQuest completedQuest in completedQuestList)
            {
                if ((!completedQuest.isHiddenFromUI && activeQuestUiList[i].text == completedQuest.questDescription))
                {
                    TextMeshProUGUI questUiToDestroy = activeQuestUiList[i];
                    activeQuestUiList.Remove(activeQuestUiList[i]);

                    Destroy(questUiToDestroy.gameObject);


                }
            }
            foreach (OJQuest missedQuest in missedQuestList)
            {
                if ((!missedQuest.isHiddenFromUI && activeQuestUiList[i].text == missedQuest.questDescription)&& !missedQuest.isRepeatable)
                {
                    TextMeshProUGUI questUiToDestroy = activeQuestUiList[i];
                    activeQuestUiList.Remove(activeQuestUiList[i]);

                    Destroy(questUiToDestroy.gameObject);

                }
            }
        }

        //for (int i = 0; i < activeQuestList.Count; i++)
        //{
        //    for (int q = 0; q < activeQuestUiList.Count; q++)
        //    {
        //        if (activeQuestList[i].questDescription == activeQuestUiList[q].text)
        //        {


        //        }
        //    }
        //}
    }

    public void ActivateQuestInteractions(OJQuest quest)
    {
        Debug.Log("Activating Quest Interactions");

        switch (quest.objective.objectiveType)
        {
            case OJQuestObjectiveType.itemBased:
                //add item interaction dialogue

                foreach (OJQuestItemObjective itemObjective in quest.objective.questItems)
                {
                    if (itemObjective.item.relatedQuests == null)
                    {
                        itemObjective.item.relatedQuests = new List<OJQuest>();
                    }

                    if (!itemObjective.item.relatedQuests.Contains(quest))
                    {
                        itemObjective.item.relatedQuests.Add(quest);
                    }

                    //if (!itemObjective.isFoodQuest)
                    //{
                        if (inventorySystem.CheckInventoryForItem(itemObjective.item) && inventorySystem.CheckItemCount(itemObjective.item) >= itemObjective.requiredAmount)
                        {
                            itemObjective.requiredAmountCollected = true;
                            AddQuestItemDialogue(quest.objective.questDialogueOptions[0], itemObjective);

                            //foreach (OJQuest itemQuest in itemObjective.item.relatedQuests)
                            //{

                            //    //EndQuest(itemQuest);
                            //}
                        }
                    //}
                    //else
                    //{
                    //    foreach (InventoryItem item in inventorySystem.inventory)
                    //    {
                    //        if (inventorySystem.CheckInventoryForItem(item) && inventorySystem.CheckIfConsumableItem(item) && inventorySystem.CheckItemCount(item) >= itemObjective.requiredAmount)

                    //        {
                    //            itemObjective.requiredAmountCollected = true;
                    //            AddQuestItemDialogue(quest.objective.questDialogueOptions[0], itemObjective);

                    //            //foreach (OJQuest itemQuest in itemObjective.item.relatedQuests)
                    //            //{

                    //            //    //EndQuest(itemQuest);
                    //            //}
                    //        }
                    //    }

                    //}
                }

                    //foreach (InventoryItem inventoryItem in FindObjectOfType<Inventory>().inventory)
                    //{
                    //    foreach (ItemInWorld.ItemInteraction interaction in inventoryItem.prefab.GetComponent<ItemInWorld>().itemInteractions)
                    //    {
                    //        if (inventoryItem.isQuestItem && !interaction.interactableObject.questInteractionDialogue.Contains(interaction.interactionDialogue))
                    //        {
                    //            interaction.interactableObject.questInteractionDialogue.Add(interaction.interactionDialogue);
                    //        }
                    //        //FindObjectOfType<PlayerDialogue>().AddQuestionForSpecificNPC(interaction.interactionDialogue, narrator);
                    //    }
                    //}

                    Debug.Log("Quest Items Activated");
                    break;
                
            case OJQuestObjectiveType.locationBased:
                foreach (Collider trigger in questColliders)
                {
                    foreach (OJQuest triggerQuest in trigger.GetComponent<OJQuestTrigger>().relatedQuests)
                    {
                        if (activeQuestList.Contains(triggerQuest) && !trigger.enabled)
                        {
                            trigger.enabled = true;
                        }
                    }
                }
                Debug.Log("Quest Triggers Activated");
                break;

            case OJQuestObjectiveType.dialogueBased:

                //if (quest.objective.isItemDialogue)
                //{
                //    foreach (OJQuestDialogue questDialogue in quest.objective.questDialogueOptions)
                //    {
                //        foreach (OJQuestItemObjective questItemObjective in quest.objective.questItems)
                //        {

                //            AddQuestItemDialogue(quest.objective.questDialogueOptions[0], questItemObjective);
                //            //playerDialogue.AddQuestionForSpecificNPC(questDialogue.questDialogueOption, questDialogue.dialogueNPCRecipient);

                            
                //        }
                //    }
                //}
                //else
                //{
                    foreach (OJQuestDialogue questDialogue in quest.objective.questDialogueOptions)
                    {
                        Debug.Log(questDialogue.questDialogueOption.name);
                        Debug.Log(questDialogue.dialogueNPCRecipient);
                        for (int i = 0; i < playerDialogue.playerQuestions.Count; i++)
                        {
                            if (playerDialogue.playerQuestions[i].npc == questDialogue.dialogueNPCRecipient && !playerDialogue.playerQuestions[i].questionsForNPC.Contains(questDialogue.questDialogueOption))
                            {
                                if (playerDialogue.playerQuestions[i].questionsForNPC.Contains(questDialogue.questDialogueOption))
                                {
                                    Debug.Log("Quest Dialogue already added");
                                }
                                else
                                {
                                    playerDialogue.AddQuestionForSpecificNPC(questDialogue.questDialogueOption, questDialogue.dialogueNPCRecipient);
                                    Debug.Log("Quest Dialogue Activated");
                                }

                                break;
                            }
                        }

                        

                    }
                //}
                break;
            case OJQuestObjectiveType.multiObjective:
                foreach (OJQuestMultiObjective multiQuest in quest.objective.childrenQuests)
                {
                    foreach (OJQuest childQuest in multiQuest.childrenQuests)
                    {
                        if (!activeQuestList.Contains(childQuest))
                        {
                            //if (!childQuest.objective.isItemDialogue)
                            //{
                            StartQuest(childQuest);
                            //}
                            //else
                            //{
                            //    // check if item is in inventory first
                            //}
                        }
                    }
                }
                break;



        }


    }

    public void AddQuestItemDialogue(OJQuestDialogue questDialogue, OJQuestItemObjective questItem)
    {
        if (inventorySystem.CheckInventoryForItem(questItem.item))
        {
            Debug.Log("quest item: '" + questItem.item.itemName + "' is in inventory");

            //OJQuestDialogue questDialogue = new OJQuestDialogue();
            //PlayerDialogueOption questDialogueOption = new PlayerDialogueOption();

            //questDialogueOption.dialogue = questDialogue.questDialogueOption.dialogue + " { " + questItem.itemName + " }";
            //questDialogueOption.isResponseToNPCDialogue = questDialogue.questDialogueOption.isResponseToNPCDialogue;
            //questDialogueOption.isLocked = false;
            //questDialogueOption.itemsToGive = new List<InventoryItem>();
            //questDialogueOption.itemsToGive.Add(questItem);
            //questDialogueOption.relatedQuests = new List<OJQuest>();
            //foreach (OJQuest quest in questDialogue.questDialogueOption.relatedQuests)
            //{
            //    questDialogueOption.relatedQuests.Add(quest);
            //}

            foreach (NPCDialogue.DialogueConnections dialogueConnection in questDialogue.dialogueNPCRecipient.npcDialogue.dialogueConnections)
            {
                if (dialogueConnection.playerDialogueInput != null)
                {
                    if (!questDialogue.questDialogueOption.dialogue.Contains(dialogueConnection.playerDialogueInput.dialogue) && !dialogueConnection.npcResponses[0].response.playerResponses.Contains(questDialogue.questDialogueOption))
                    {
                        dialogueConnection.npcResponses[0].response.playerResponses.Add(questDialogue.questDialogueOption);
                    }
                }
            }

            playerDialogue.AddQuestionForSpecificNPC(questDialogue.questDialogueOption, questDialogue.dialogueNPCRecipient);

            Debug.Log("Quest Item: '" + questItem.item.itemName + "' dialogue has been added");

        }
        else
        {
            Debug.Log("quest item: '" + questItem.item.itemName + "' is NOT in inventory");


            RemoveQuestItemDialogue(questDialogue, questItem);


        }
    }

    public void RemoveQuestItemDialogue(OJQuestDialogue questDialogue, OJQuestItemObjective questItem)
    {
        if (!inventorySystem.CheckInventoryForItem(questItem.item) || (inventorySystem.CheckInventoryForItem(questItem.item) && inventorySystem.CheckItemCount(questItem.item) < questItem.requiredAmount))
        {
            foreach (PlayerDialogue.PlayerQuestions questions in playerDialogue.playerQuestions)
            {
                if (questions.npc == questDialogue.dialogueNPCRecipient)
                {
                    foreach (PlayerDialogueOption playerDialogueOption in questions.questionsForNPC)
                    {
                        if (playerDialogueOption.dialogue == questDialogue.questDialogueOption.dialogue)
                        {
                            //questDialogue.questDialogueOption.isLocked = true;
                            playerDialogue.RemoveDialogueForSpecificNPC(questDialogue.questDialogueOption, questDialogue.dialogueNPCRecipient);

                            Debug.Log("Item dialogue removed");
                        }
                    }
                }
            }
        }
    }

   
    public void DeactivateOldQuestInteractions()
    {
        foreach (OJQuest quest in activeQuestList)
        {
            if (!quest.questStarted || quest.questEnded)
            {
                switch (quest.objective.objectiveType)
                {
                    case OJQuestObjectiveType.itemBased:
                        //add item interaction dialogue
                        foreach (OJQuestItemObjective itemObjective in quest.objective.questItems)
                        {
                            if (itemObjective.item.relatedQuests == null)
                            {
                                itemObjective.item.relatedQuests = new List<OJQuest>();
                            }
                            else
                            {
                                itemObjective.item.relatedQuests.Remove(quest);
                            }
                        }

                        //foreach (InventoryItem inventoryItem in FindObjectOfType<Inventory>().inventory)
                        //{
                        //    foreach (ItemInWorld.ItemInteraction interaction in inventoryItem.prefab.GetComponent<ItemInWorld>().itemInteractions)
                        //    {
                        //        if (!inventoryItem.isQuestItem && interaction.interactableObject.questInteractionDialogue.Contains(interaction.interactionDialogue))
                        //        {
                        //            interaction.interactableObject.questInteractionDialogue.Remove(interaction.interactionDialogue);
                        //        }
                        //        //FindObjectOfType<PlayerDialogue>().AddQuestionForSpecificNPC(interaction.interactionDialogue, narrator);
                        //    }
                        //}
                        break;

                    case OJQuestObjectiveType.locationBased: // needs a monobehaviour to store colliders per location quest
                        foreach (Collider trigger in questColliders)
                        {
                            foreach (OJQuest triggerQuest in trigger.GetComponent<OJQuestTrigger>().relatedQuests)
                            {
                                if ((completedQuestList.Contains(triggerQuest) || missedQuestList.Contains(triggerQuest)) && !activeQuestList.Contains(triggerQuest) && trigger.enabled)
                                {
                                    trigger.enabled = false;
                                }
                            }

                        }
                        break;

                    case OJQuestObjectiveType.dialogueBased:
                        foreach (OJQuestDialogue questDialogue in quest.objective.questDialogueOptions)
                        {
                            playerDialogue.AddQuestionForSpecificNPC(questDialogue.questDialogueOption, questDialogue.dialogueNPCRecipient);
                        }
                        break;
                }
            }
        }
    }

    public void EndQuest(OJQuest quest)
    {
        //remove quest from active quest list
        //add quest to past quest list

        if (quest.questStarted && !quest.questEnded)
        {
            if (quest.objective.objectiveType == OJQuestObjectiveType.dialogueBased)
            {
                foreach (OJQuestDialogue questDialogue in quest.objective.questDialogueOptions)
                {
                    playerDialogue.RemoveDialogueForSpecificNPC(questDialogue.questDialogueOption, questDialogue.dialogueNPCRecipient);
                    //quest.outcome.unlockDialogue.RemoveDialogueForSpecificNPC(FindObjectOfType<PlayerDialogue>(), questDialogue.dialogueNPCRecipient, questDialogue.questDialogueOption);
                }
            }
            else if (quest.objective.objectiveType == OJQuestObjectiveType.itemBased)
            {
                foreach (OJQuestItemObjective questItem in quest.objective.questItems)
                {
                    if (!questItem.questCompleted)
                    {
                        questItem.questCompleted = true;
                    }
                    
                }
            }
            else if (quest.objective.objectiveType == OJQuestObjectiveType.multiObjective)
            {
                quest.objective.childrenQuests[0].completedChildrenQuestCount = 0;

                foreach(OJQuest childQuest in quest.objective.childrenQuests[0].childrenQuests)
                {
                    if (childQuest.questEnded)
                    {
                        quest.objective.childrenQuests[0].completedChildrenQuestCount += 1;
                    }
                }

                if (quest.objective.childrenQuests[0].completedChildrenQuestCount >= quest.objective.childrenQuests[0].requiredChildrenQuestsCompletedToComplete)
                {
                    quest.questEnded = true;
                }
            }

            if (activeQuestList.Contains(quest))
            {
                activeQuestList.Remove(quest);
            }

            if (!missedQuestList.Contains(quest) && !completedQuestList.Contains(quest))
            {
                completedQuestList.Add(quest);

                playerInfoController.AffectStatValues(quest.outcome.statsToEffectList);

                foreach (OJQuest missedQuest in quest.outcome.questsToLock)
                {
                    activeQuestList.Remove(missedQuest);
                    missedQuestList.Add(missedQuest);

                    missedQuest.questEnded = true;
                }
            }

            if (quest.outcome.questsToUnlock.Count > 0)
            {
                foreach (OJQuest nextQuest in quest.outcome.questsToUnlock)
                {
                    //activeQuestList.Add(nextQuest);
                    StartQuest(nextQuest);
                }
            }

            

            quest.questEnded = true;

            DeactivateOldQuestInteractions();

            RemoveInactiveQuestUI();

        }

        //for (int q = 0; q < activeQuestList.ToArray().Length; q++)
        //{
        //    //int completedChildrenQuestCount = 0;
        //    if (activeQuestList[q].objective.objectiveType == OJQuestObjectiveType.multiObjective)
        //    {
        //        for (int aQ = 0; aQ < activeQuestList[q].objective.childrenQuests.Count; aQ++)
        //        {
        //            activeQuestList[q].objective.childrenQuests[aQ].completedChildrenQuestCount = 0;
        //            for (int cQ = 0; cQ < activeQuestList[q].objective.childrenQuests[aQ].childrenQuests.Count; cQ++) 
        //            {
        //                //if (activeQuestList.Contains(activeQuestList[q].objective.childrenQuests[aQ].childrenQuests[cQ]) || activeQuestList[q].objective.childrenQuests[aQ].childrenQuests[cQ].questStarted && !activeQuestList[q].objective.childrenQuests[aQ].childrenQuests[cQ].questEnded)
        //                //{
        //                //    //EndQuest(activeQuest.objective.childrenQuests[i]);
        //                //    break;
        //                //}
                        
        //                if (activeQuestList[q].objective.childrenQuests[aQ].childrenQuests[cQ].questEnded)
        //                {
        //                    activeQuestList[q].objective.childrenQuests[aQ].completedChildrenQuestCount ++;
        //                }

                        

        //                //if (aQ + 1 >= activeQuestList[q].objective.childrenQuests.Count)
        //                //{
        //                //    EndQuest(activeQuestList[q]);
        //                //}
        //            }

        //            if (activeQuestList[q].objective.childrenQuests[aQ].completedChildrenQuestCount >= activeQuestList[q].objective.childrenQuests[aQ].requiredChildrenQuestsCompletedToComplete)
        //            {
        //                EndQuest(activeQuestList[q]);
        //                RemoveInactiveQuestUI();
        //                //Debug.Log(activeQuestList[q].questID + " Quest has ended");
        //            }
        //        }

        //    }
        //}

        //if (quest.isRepeatable)
        //{
        //    quest.questStarted = false;
        //    quest.questEnded = false;

        //    if (quest.objective.objectiveType == OJQuestObjectiveType.itemBased)
        //    {
        //        foreach (OJQuestItemObjective questItem in quest.objective.questItems)
        //        {
        //            questItem.questCompleted = false;

        //            if (inventorySystem.CheckInventoryForItem(questItem.item) && inventorySystem.CheckItemCount(questItem.item) >= questItem.requiredAmount)
        //            {
        //                questItem.requiredAmountCollected = true;

        //            }
        //            else
        //            {
        //                questItem.requiredAmountCollected = false;
        //            }
        //        }
        //    }
        //}


    }
}
