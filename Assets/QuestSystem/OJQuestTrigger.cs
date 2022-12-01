using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Allows items in the scene to be linked to quest

public class OJQuestTrigger : MonoBehaviour
{
    private OJQuestManager questManager;
    public List<OJQuest> relatedQuests;

    public string interactionObjectName;

    public List<PlayerDialogueOption> questInteractionDialogue;

    public List<EnvironmentalItemInteraction> itemInteractionsList;

    private void Start()
    {
        questManager = FindObjectOfType<OJQuestManager>();

        //questInteractionDialogue = new List<PlayerDialogueOption>();

    }

    public void OnTriggerEnter(Collider other)
    {
        foreach (OJQuest quest in relatedQuests)
        {
            if (questManager.activeQuestList.Contains(quest))
            {
                questManager.EndQuest(quest);
            }
        }
    }

    //add dialogue options for each item interaction
    public bool CheckItemsInInventory()
    {
        // create dialogue option for each applicable key in inventory
        foreach (InventoryItem item in FindObjectOfType<Inventory>().inventory)
        {
            for (int i = 0; i < itemInteractionsList.Count; i++)
            {
                if (itemInteractionsList[i].item == item)
                {
                    return true;
                }
            }
        }

        return false;
    }
}

[System.Serializable]
public class EnvironmentalItemInteraction
{
    public InventoryItem item;

    public List<UnityEvent> itemInteractionEvents;
    public List<StatContainer.Stat> statsToEffectList;
}
