using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OJQuestInteraction : MonoBehaviour
{
    private QuestManager questManager;
    public OJQuest quest;

    public List<PlayerDialogueOption> questInteractionDialogue;

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();

        questInteractionDialogue = new List<PlayerDialogueOption>();

    }

    public void OnTriggerEnter(Collider other)
    {
        if (questManager.activeQuestList.Contains(quest))
        {
            questManager.EndQuest(quest);
        }
    }
}
