using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(menuName = "QuestSystem/Quest")]
public class OJQuest : ScriptableObject
{
    [HideInInspector] public QuestManager questManager;
    [HideInInspector] public PlayerDialogue playerDialogue;

    private void OnEnable()
    {
         questManager = FindObjectOfType<QuestManager>();
         playerDialogue = FindObjectOfType<PlayerDialogue>();


    }

    public string questID;

    [TextArea (3,10)] public string questDescription;

    public OJQuestObjective objective;

    public OJQuestOutcome outcome;

    // objective
    // subsiquent objectives
    // sin affects & environmental changes



}

[System.Serializable]
public enum OJQuestType { itemBased, locationBased, dialogueBased }

[System.Serializable]
public class OJQuestObjective
{
    public OJQuestType questType;

    public List<ItemInWorld> questItems;
    public List<Collider> questLocationTriggers;
    public List<PlayerDialogueOption> questDialogueOptions;

}

[System.Serializable]
public class OJQuestOutcome
{
    // lock & unlock dialogue
    public UnlockNewDialogue newDialogue;

    public void UnlockDialogue(PlayerDialogue playerDialogue, NPCInfo npc, PlayerDialogueOption dialogueToUnlock)
    {

        newDialogue.UnlockDialogueForSpecificNPC(playerDialogue, npc, dialogueToUnlock);
    }

    public void LockDialogue(PlayerDialogue playerDialogue, NPCInfo npc, PlayerDialogueOption dialogueToLock)
    {
        //newDialogue.UnlockDialogueForSpecificNPC(playerDialogue, npc, dialogueToLock);

    }

    // add & remove quests
    public List<OJQuest> questsToUnlock;
    public List<OJQuest> questsToLock;

    // cause environmental effects
    public EnvironmentalChangeController environmentalChangeController;

}

//[CustomEditor(typeof(OJQuest))]
//public class OJQuestObjectiveEditor : Editor
//{
//    OJQuestObjective questObjective;
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        OJQuest quest = (OJQuest)target;
//        if (quest == null) return;

//        questObjective = quest.objective;

//        switch (questObjective.questType)
//        {
//            case OJQuestType.itemBased:
//                break;
//            case OJQuestType.locationBased:
//                break;
//            case OJQuestType.dialogueBased:
//                break;
//        }


        
//    }
//}
