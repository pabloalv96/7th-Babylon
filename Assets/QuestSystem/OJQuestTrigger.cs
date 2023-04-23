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

    public List<UnityEvent> conditionalEvents;

    public AudioClip audioClip;


    private void Awake()
    {
        questManager = FindObjectOfType<OJQuestManager>();

        //questInteractionDialogue = new List<PlayerDialogueOption>();

    }

    private void Update()
    {
        if (relatedQuests.Count > 0 && relatedQuests[0].questEnded)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            foreach (OJQuest quest in relatedQuests)
            {
                if (!quest.questStarted)
                {
                    questManager.StartQuest(quest);
                }
                else if (quest.questStarted && !quest.questEnded)
                {
                    questManager.EndQuest(quest);
                }
            }

            foreach (UnityEvent conditional in conditionalEvents)
            {
                conditional.Invoke();
            }

            if (audioClip != null)
            {
                if (audioClip != null)
                {
                    if (questManager.audioSource.isPlaying)
                    {
                        questManager.audioSource.Stop();
                    }

                    questManager.audioSource.PlayOneShot(audioClip);
                }
            }
        }
    }

    //add dialogue options for each item interaction
    //public bool CheckItemsInInventory()
    //{
    //    // create dialogue option for each applicable key in inventory
    //    foreach (InventoryItem item in FindObjectOfType<Inventory>().inventory)
    //    {
    //        for (int i = 0; i < itemInteractionsList.Count; i++)
    //        {
    //            if (itemInteractionsList[i].item == item)
    //            {
    //                return true;
    //            }
    //        }
    //    }

    //    return false;
    //}
}

[System.Serializable]
public class EnvironmentalItemInteraction
{
    public InventoryItem item;

    public List<UnityEvent> itemInteractionEvents;
    public List<StatContainer.Stat> statsToEffectList;
}
