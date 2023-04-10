using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(OJQuestManager))]
public class QuestManagerInspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        OJQuestManager questManager = (OJQuestManager)target;
        if (questManager == null) return;

        GUILayout.Space(5f);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Reset Quests and Dialogue"))
        {
            ResetQuestsAndDialogue(questManager);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5f);
    }

    public void ResetQuestsAndDialogue(OJQuestManager questManager)
    {
        foreach (OJQuest quest in questManager.listOfAllQuests)
        {
            quest.questStarted = false;
            quest.questEnded = false;

            if (quest.objective.objectiveType == OJQuestObjectiveType.itemBased)
            {
                foreach (OJQuestItemObjective item in quest.objective.questItems)
                {
                    item.questCompleted = false;
                    item.requiredAmountCollected = false;
                }
            }
        }

        foreach (PlayerDialogueOption dialogueOption in questManager.lockedAtStartDialogue)
        {
            dialogueOption.isLocked = true;
        }

        foreach (PlayerDialogueOption dialogueOption in questManager.unlockedAtStartDialogue)
        {
            dialogueOption.isLocked = false;
        }
    }
}


#endif
