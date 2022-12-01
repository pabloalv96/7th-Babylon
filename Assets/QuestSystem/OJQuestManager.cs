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

    // track currently active quests
    // lock quests that are completed or no longer available

    private void Awake()
    {
        questColliders = new List<Collider>();

        foreach(OJQuestTrigger questInScene in FindObjectsOfType<OJQuestTrigger>())
        {
            if (questInScene.GetComponent<Collider>() && questInScene.GetComponent<Collider>().isTrigger)
            {
                questColliders.Add(questInScene.GetComponent<Collider>());
            }
        }
    }

    public void StartQuest(OJQuest quest)
    {
        // add quest to active quest list
        if (!quest.questStarted && !quest.questEnded)
        {
            if (!activeQuestList.Contains(quest))
            {
                ActivateQuestInteractions(quest);

                activeQuestList.Add(quest);

                TextMeshProUGUI questUIText = Instantiate(questUIPrefab, activeQuestUIParent.transform);
                questUIText.text = quest.questDescription;

                activeQuestUiList.Add(questUIText);
            }

            

            quest.questStarted = true;
        }

        // create quest UI
    }

    public void RemoveInactiveQuestUI()
    {

        for (int q = 0; q < activeQuestUiList.Count; q++)
        {
            foreach (OJQuest completedQuest in completedQuestList)
            {
                if (activeQuestUiList[q].text == completedQuest.questDescription)
                {
                    Destroy(activeQuestUiList[q].gameObject);
                    activeQuestUiList.Remove(activeQuestUiList[q]);

                }
            }
            foreach (OJQuest missedQuest in missedQuestList)
            {
                if (activeQuestUiList[q].text == missedQuest.questDescription)
                {
                    Destroy(activeQuestUiList[q].gameObject);
                    activeQuestUiList.Remove(activeQuestUiList[q]);

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
            case OJQuestType.locationBased:
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
            case OJQuestType.dialogueBased:
                if (quest.objective.isItemDialogue)
                {
                    foreach (InventoryItem item in quest.objective.questItems)
                    {
                        OJQuestDialogue questDialogue = new OJQuestDialogue();

                        questDialogue.questDialogueOption.dialogue = quest.objective.questDialogueOptions[0].questDialogueOption.dialogue + " \n { Give " + item.itemName + " }";

                        FindObjectOfType<PlayerDialogue>().AddQuestionForSpecificNPC(questDialogue.questDialogueOption, questDialogue.dialogueNPCRecipient);
                    }
                }
                else
                {
                    foreach (OJQuestDialogue questDialogue in quest.objective.questDialogueOptions)
                    {
                        FindObjectOfType<PlayerDialogue>().AddQuestionForSpecificNPC(questDialogue.questDialogueOption, questDialogue.dialogueNPCRecipient);
                    }
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

                    case OJQuestType.locationBased: // needs a monobehaviour to store colliders per location quest
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

                    case OJQuestType.dialogueBased:
                        foreach (OJQuestDialogue questDialogue in quest.objective.questDialogueOptions)
                        {
                            FindObjectOfType<PlayerDialogue>().AddQuestionForSpecificNPC(questDialogue.questDialogueOption, questDialogue.dialogueNPCRecipient);
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
                case OJQuestType.locationBased: // needs a monobehaviour to store colliders per location quest
                    //foreach (Collider trigger in quest.objective.questLocationTriggers)
                    //{
                    //    trigger.enabled = false;
                    //}
                    break;
                case OJQuestType.dialogueBased:
                    foreach (OJQuestDialogue questDialogue in quest.objective.questDialogueOptions)
                    {
                        FindObjectOfType<PlayerDialogue>().RemoveDialogueForSpecificNPC(questDialogue.questDialogueOption, questDialogue.dialogueNPCRecipient);
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

                FindObjectOfType<PlayerInfoController>().AffectStatValues(quest.outcome.statsToEffectList);

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
                    //activeQuestList.Add(nextQuest);
                    StartQuest(nextQuest);
                }
            }

            DeactivateOldQuestInteractions();

            RemoveInactiveQuestUI();

            quest.questEnded = true;

        }

    }
}
