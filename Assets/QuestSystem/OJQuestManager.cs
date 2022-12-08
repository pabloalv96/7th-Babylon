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

    private DialogueListSystem dialogueSystem;
    //private DialogueInitiator dialogueInitiator;
    //private OJQuestManager questManager;
    private Inventory inventorySystem;
    private PlayerDialogue playerDialogue;
    private PlayerInfoController playerInfoController;

    // track currently active quests
    // lock quests that are completed or no longer available

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
        if (!quest.questStarted && !quest.questEnded && !activeQuestList.Contains(quest))
        {
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

            if (quest.objective.objectiveType == OJQuestObjectiveType.dialogueBased && quest.objective.isItemDialogue)
            {
                foreach (OJQuestItemObjective itemObjective in quest.objective.questItems)
                {
                    if (inventorySystem.CheckInventoryForItem(itemObjective.questItem) && inventorySystem.CheckItemCount(itemObjective.questItem) >= itemObjective.requiredAmount)
                    {
                        foreach (OJQuest itemQuest in itemObjective.questItem.relatedQuests)
                        {
                            EndQuest(itemQuest);
                        }
                    }
                }
            }

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
                if (!completedQuest.isHiddenFromUI && activeQuestUiList[i].text == completedQuest.questDescription)
                {
                    TextMeshProUGUI questUiToDestroy = activeQuestUiList[i];
                    activeQuestUiList.Remove(activeQuestUiList[i]);

                    Destroy(questUiToDestroy.gameObject);


                }
            }
            foreach (OJQuest missedQuest in missedQuestList)
            {
                if (!missedQuest.isHiddenFromUI && activeQuestUiList[i].text == missedQuest.questDescription)
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
                    if (itemObjective.questItem.relatedQuests == null)
                    {
                        itemObjective.questItem.relatedQuests = new List<OJQuest>();
                    }

                    if (!itemObjective.questItem.relatedQuests.Contains(quest))
                    {
                        itemObjective.questItem.relatedQuests.Add(quest);
                    }

                    if (inventorySystem.CheckInventoryForItem(itemObjective.questItem))
                    {
                        foreach (OJQuest itemQuest in itemObjective.questItem.relatedQuests)
                        {
                            EndQuest(itemQuest);
                        }
                    }
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

                if (quest.objective.isItemDialogue)
                {
                    foreach (OJQuestDialogue questDialogue in quest.objective.questDialogueOptions)
                    {
                        foreach (OJQuestItemObjective questItemObjective in quest.objective.questItems)
                        {

                            AddQuestItemDialogue(quest.objective.questDialogueOptions[0], questItemObjective.questItem);
                            //playerDialogue.AddQuestionForSpecificNPC(questDialogue.questDialogueOption, questDialogue.dialogueNPCRecipient);

                            
                        }
                    }
                }
                else
                {
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
                }
                break;
            case OJQuestObjectiveType.multiObjective:
                foreach (OJQuest childQuest in quest.objective.childrenQuests)
                {
                    if (!activeQuestList.Contains(childQuest))
                    {
                        if (!childQuest.objective.isItemDialogue)
                        {
                            StartQuest(childQuest);
                        }
                        //else
                        //{
                        //    // check if item is in inventory first
                        //}
                    }
                }
                break;



        }


    }

    public void AddQuestItemDialogue(OJQuestDialogue questDialogue, InventoryItem questItem)
    {
        if (inventorySystem.CheckInventoryForItem(questItem))
        {
            Debug.Log("quest item: '" + questItem.itemName + "' is in inventory");

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

            Debug.Log("Quest Item: '" + questItem.itemName + "' dialogue has been added");

        }
        else
        {
            Debug.Log("quest item: '" + questItem.itemName + "' is NOT in inventory");

            RemoveQuestItemDialogue(questDialogue, questItem);


        }
    }

    public void RemoveQuestItemDialogue(OJQuestDialogue questDialogue, InventoryItem questItem)
    {
        if (!inventorySystem.CheckInventoryForItem(questItem))
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

                        }
                    }
                }
            }
        }
    }

    //public void ()
    //{

    //}

    //public void TriggerQuestInteraction(OJQuest quest)
    //{
    //    switch (quest.objective.questType)
    //    {
    //        case OJQuestType.itemBased:
    //            //add item interaction dialogue
    //            break;
    //        case OJQuestType.locationBased:
    //            foreach (Collider trigger in quest.objective.questLocationTriggers)
    //            {
    //                trigger.enabled = false;
    //            }
    //            break;
    //        case OJQuestType.dialogueBased:
    //            foreach (OJQuestDialogue questDialogue in quest.objective.questDialogueOptions)
    //            {
    //                quest.outcome.unlockDialogue.RemoveDialogueForSpecificNPC(FindObjectOfType<PlayerDialogue>(), questDialogue.dialogueNPCRecipient, questDialogue.questDialogueOption);
    //            }
    //            break;
    //    }
    //}

    //public bool CheckItemsInInventory(OJQuest quest)
    //{
    //    if (quest.objective.questType == OJQuestType.dialogueBased && quest.objective.isItemDialogue)
    //    {
    //        // create dialogue option for each applicable key in inventory
    //        foreach (InventoryItem item in inventorySystem.inventory)
    //        {
    //            Debug.Log(item.name);

    //            for (int i = 0; i < quest.objective.questItems.Count; i++)
    //            {
    //                if (quest.objective.questItems[i] == item)
    //                {
    //                    return true;
    //                }
    //            }
    //        }
    //    }

    //    Debug.Log("Inventory has been checked");

    //    return false;
    //}

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
                            if (itemObjective.questItem.relatedQuests == null)
                            {
                                itemObjective.questItem.relatedQuests = new List<OJQuest>();
                            }
                            else
                            {
                                itemObjective.questItem.relatedQuests.Remove(quest);
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
            switch (quest.objective.objectiveType)
            {
                case OJQuestObjectiveType.itemBased:

                    break;
                case OJQuestObjectiveType.locationBased: // needs a monobehaviour to store colliders per location quest
                    //foreach (Collider trigger in quest.objective.questLocationTriggers)
                    //{
                    //    trigger.enabled = false;
                    //}
                    break;
                case OJQuestObjectiveType.dialogueBased:
                    foreach (OJQuestDialogue questDialogue in quest.objective.questDialogueOptions)
                    {
                        playerDialogue.RemoveDialogueForSpecificNPC(questDialogue.questDialogueOption, questDialogue.dialogueNPCRecipient);
                        //quest.outcome.unlockDialogue.RemoveDialogueForSpecificNPC(FindObjectOfType<PlayerDialogue>(), questDialogue.dialogueNPCRecipient, questDialogue.questDialogueOption);
                    }
                    break;
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

            DeactivateOldQuestInteractions();

            RemoveInactiveQuestUI();

            quest.questEnded = true;


        }

        for (int q = 0; q < activeQuestList.ToArray().Length; q++)

        {
            if (activeQuestList[q].objective.objectiveType == OJQuestObjectiveType.multiObjective)
            {
                for (int i = 0; i < activeQuestList[q].objective.childrenQuests.Count; i++)
                {
                    if (activeQuestList.Contains(activeQuestList[q].objective.childrenQuests[i]))
                    {
                        //EndQuest(activeQuest.objective.childrenQuests[i]);
                        break;
                    }
                    if (i + 1 >= activeQuestList[q].objective.childrenQuests.Count)
                    {
                        EndQuest(activeQuestList[q]);
                    }
                }



            }
        }


    }
}
