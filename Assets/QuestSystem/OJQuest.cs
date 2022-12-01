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

    public bool requiresDialogueDecision;

    public bool questStarted, questEnded;

    private void OnEnable()
    {
         questManager = FindObjectOfType<OJQuestManager>();
         playerDialogue = FindObjectOfType<PlayerDialogue>();
         environmentalChanges = FindObjectOfType<EnvironmentalChangeController>();
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

    public bool isItemDialogue;
    public List<InventoryItem> questItems;
    public List<OJQuestDialogue> questDialogueOptions;

}

[System.Serializable]
public class OJQuestDialogue
{
    public PlayerDialogueOption questDialogueOption;
    public NPCInfo dialogueNPCRecipient;
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
