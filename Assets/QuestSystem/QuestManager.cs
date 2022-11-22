using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
   // public TextMeshProUGUI questUIPrefab;

    public List<OJQuest> activeQuestList;

    public List<OJQuest> completedQuestList;
    public List<OJQuest> missedQuestList;

    public NPCInfo narrator;

    // track currently active quests
    // lock quests that are completed or no longer available

    public void StartQuest(OJQuest quest)
    {
        // add quest to active quest list
        if (!quest.questStarted && !quest.questEnded)
        {
            if (!activeQuestList.Contains(quest))
            {
                ActivateQuestInteractions(quest);

                activeQuestList.Add(quest);
            }

            

            quest.questStarted = true;
        }

        // create quest UI
    }

    public void ActivateQuestInteractions(OJQuest quest)
    {
        Debug.Log("Activating Quest Interactions");

        switch (quest.objective.questType)
        {
            case OJQuestType.itemBased:
                //add item interaction dialogue

                foreach (InventoryItem item in quest.objective.questItems)
                {
                    if (!item.isQuestItem)
                    {
                        item.isQuestItem = true;
                        item.relatedQuest = quest;
                    }
                }

                foreach (InventoryItem inventoryItem in FindObjectOfType<Inventory>().inventory)
                {
                    foreach (ItemInWorld.ItemInteraction interaction in inventoryItem.prefab.GetComponent<ItemInWorld>().itemInteractions)
                    {
                        if (inventoryItem.isQuestItem && !interaction.interactableObject.questInteractionDialogue.Contains(interaction.interactionDialogue))
                        {

                            interaction.interactableObject.questInteractionDialogue.Add(interaction.interactionDialogue);
                        }
                        //FindObjectOfType<PlayerDialogue>().AddQuestionForSpecificNPC(interaction.interactionDialogue, narrator);
                    }
                }

                Debug.Log("Quest Items Activated");
                break;
            case OJQuestType.locationBased:
                foreach (Collider trigger in quest.objective.questLocationTriggers)
                {
                    trigger.enabled = true;
                }
                Debug.Log("Quest Triggers Activated");
                break;
            case OJQuestType.dialogueBased:
                foreach (OJQuestDialogue questDialogue in quest.objective.questDialogueOptions)
                {
                    quest.outcome.unlockDialogue.UnlockDialogueForSpecificNPC(FindObjectOfType<PlayerDialogue>(), questDialogue.dialogueNPCRecipient, questDialogue.questDialogueOption);
                }
                Debug.Log("Quest Dialogue Activated");
                break;
        }

        
    }

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

    public void DeactivateOldQuestInteractions()
    {
        foreach (OJQuest quest in activeQuestList)
        {
            if (!quest.questStarted || quest.questEnded)
            {
                switch (quest.objective.questType)
                {
                    case OJQuestType.itemBased:
                        //add item interaction dialogue
                        foreach (InventoryItem item in quest.objective.questItems)
                        {
                            if (item.isQuestItem)
                            {
                                item.isQuestItem = false;
                                item.relatedQuest = null;
                            }
                        }

                        foreach (InventoryItem inventoryItem in FindObjectOfType<Inventory>().inventory)
                        {
                            foreach (ItemInWorld.ItemInteraction interaction in inventoryItem.prefab.GetComponent<ItemInWorld>().itemInteractions)
                            {
                                if (!inventoryItem.isQuestItem && interaction.interactableObject.questInteractionDialogue.Contains(interaction.interactionDialogue))
                                {
                                    interaction.interactableObject.questInteractionDialogue.Remove(interaction.interactionDialogue);
                                }
                                //FindObjectOfType<PlayerDialogue>().AddQuestionForSpecificNPC(interaction.interactionDialogue, narrator);
                            }
                        }
                        break;

                    case OJQuestType.locationBased:
                        foreach (Collider trigger in quest.objective.questLocationTriggers)
                        {
                            trigger.enabled = true;
                        }
                        break;

                    case OJQuestType.dialogueBased:
                        foreach (OJQuestDialogue questDialogue in quest.objective.questDialogueOptions)
                        {
                            quest.outcome.unlockDialogue.UnlockDialogueForSpecificNPC(FindObjectOfType<PlayerDialogue>(), questDialogue.dialogueNPCRecipient, questDialogue.questDialogueOption);
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
            switch (quest.objective.questType)
            {
                case OJQuestType.itemBased:

                    break;
                case OJQuestType.locationBased:
                    foreach (Collider trigger in quest.objective.questLocationTriggers)
                    {
                        trigger.enabled = false;
                    }
                    break;
                case OJQuestType.dialogueBased:
                    foreach (OJQuestDialogue questDialogue in quest.objective.questDialogueOptions)
                    {
                        quest.outcome.unlockDialogue.RemoveDialogueForSpecificNPC(FindObjectOfType<PlayerDialogue>(), questDialogue.dialogueNPCRecipient, questDialogue.questDialogueOption);
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

                foreach (OJQuest missedQuest in quest.outcome.questsToLock)
                {
                    activeQuestList.Remove(missedQuest);
                    missedQuestList.Add(missedQuest);

                    missedQuest.questEnded = true;
                }
            }
            
            if (quest.outcome.questsToUnlock.Count > 0)
            {
                foreach(OJQuest nextQuest in quest.outcome.questsToUnlock)
                {
                    activeQuestList.Add(nextQuest);
                    StartQuest(nextQuest);
                }
            }

            DeactivateOldQuestInteractions();

            quest.questEnded = true;

        }
    }
}
