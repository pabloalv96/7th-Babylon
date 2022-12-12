using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(menuName = "QuestSystem/Quest")]
public class OJQuest : ScriptableObject
{
    [HideInInspector] public OJQuestManager questManager;
    [HideInInspector] public PlayerDialogue playerDialogue;
    [HideInInspector] public EnvironmentalChangeController environmentalChanges;

    public bool isHiddenFromUI;

    public bool questStarted, questEnded, isRepeatable = false;

    private void OnEnable()
    {
         questManager = FindObjectOfType<OJQuestManager>();
         playerDialogue = FindObjectOfType<PlayerDialogue>();
         environmentalChanges = FindObjectOfType<EnvironmentalChangeController>();
    }

    public string questID;

    [TextArea (3,10)] public string questDescription;

    public OJQuestActivation questActivation;

    public OJQuestObjective objective;

    public OJQuestOutcome outcome;

    // objective
    // subsiquent objectives
    // sin affects & environmental changes

}

[System.Serializable]
public class OJQuestActivation
{
    public List<OJQuest> questsToLock;
}

[System.Serializable]
public enum OJQuestObjectiveType { itemBased, locationBased, dialogueBased, multiObjective }

[System.Serializable]
public class OJQuestObjective
{
    public OJQuestObjectiveType objectiveType;

    //public bool isItemDialogue;
    public List<OJQuestItemObjective> questItems;
    public List<OJQuestDialogue> questDialogueOptions;
    public List<OJQuestMultiObjective> childrenQuests;

}

[System.Serializable]
public class OJQuestDialogue
{
    public PlayerDialogueOption questDialogueOption;
    public NPCInfo dialogueNPCRecipient;
}

[System.Serializable]
public class OJQuestMultiObjective
{
    public int requiredChildrenQuestsCompletedToComplete;
    public int completedChildrenQuestCount;

    public List<OJQuest> childrenQuests;
}

[System.Serializable]
public class OJQuestOutcome
{
    // add & remove quests
    public List<OJQuest> questsToUnlock;
    public List<OJQuest> questsToLock;

    // cause environmental effects
    public List<StatContainer.Stat> statsToEffectList;

}

[System.Serializable]
public class OJQuestItemObjective
{
    // add minAmount for sloth / greed / gluttony and disappointed npc dialogue if the player returns less than required

    public int requiredAmount;

    //public bool isFoodQuest;

    public InventoryItem item;

    public bool requiredAmountCollected;
    public bool questCompleted;
}


//[CustomEditor(typeof(OJQuest))]
//public class OJQuestObjectiveEditor : Editor
//{
//    OJQuestObjective questObjective;
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        //OJQuest quest = (OJQuest)target;
//        //if (quest == null) return;

//        //questObjective = quest.objective;

//        //switch (questObjective.questType)
//        //{
//        //    case OJQuestType.itemBased:
//        //        break;
//        //    case OJQuestType.locationBased:
//        //        break;
//        //    case OJQuestType.dialogueBased:
//        //        break;
//        //}

//        if (Application.isPlaying && Application.isEditor)
//        {
//            DirectoryInfo dir = new DirectoryInfo("Assets/QuestSystem/Quests");
//            FileInfo[] info = dir.GetFiles("*.asset");

//            foreach(FileInfo f in info)
//            {
//                if (f.)
//            }

//            //string[] files = System.IO.Directory.GetFiles("Assets/QuestSystem/Quests");
//            //foreach (string file in files)
//            //{
//            //    //Do work on the files here
//            //    if (file.GetType())
//            //}

//        }

//    }
//}
