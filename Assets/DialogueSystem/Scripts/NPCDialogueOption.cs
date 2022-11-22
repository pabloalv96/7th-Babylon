using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;


[CreateAssetMenu(menuName =("DialogueSystem/NPCDialogue"))]
public class NPCDialogueOption : ScriptableObject
{
    [TextArea(3, 10)]
    public string dialogue; // set dialogue text

    public bool toOtherNPC = false;

    public bool requiresResponse = true;   // Set to false to disable player dialogue selection

    public NPCDialogueOption continuedDialogue; //Set another dialogue option to follow this
                                                //**only if requiresResponse is false

    public List<PlayerDialogueOption> playerResponses; //Set player dialogue options to respond with

    public bool limitedTime; // If true player will need to respond within a time limit, when the time runs out the currently selected option will be said
    public float timeLimit;

    public bool playerCanChangeTopic = true; //if true the player will be able to change the topic
                                             //**unless there is a time limit

    public bool playerCanLeaveDialogue = true;//if true the player will be able to leave the dialogue
                                       //**unless there is a time limit

    public bool endOfConversation;
    public bool changeOfTopic; // forced topic change by NPC

    public List<UnityEvent> conditionalEvents;

    public bool isQuestPrompt;
    public OJQuest relatedQuest;


}

//#if UNITY_EDITOR
//[CustomEditor(typeof(NPCDialogueOption))]
//public class NPCDialogueOptionEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        NPCDialogueOption dialogueOption = (NPCDialogueOption)target;
//        if (dialogueOption == null) return;

//        GUILayout.Toggle(dialogueOption.toOtherNPC, "ToOtherNPC");

//        if (dialogueOption.toOtherNPC)
//        {
//            dialogueOption.continuedDialogue = (NPCDialogueOption)EditorGUILayout.ObjectField(dialogueOption.continuedDialogue, typeof(NPCDialogueOption), true);
//        }

//        GUILayout.Toggle(dialogueOption.requiresResponse, "RequiresResponse");

//        if (dialogueOption.requiresResponse)
//        {
//            dialogueOption.playerResponses = (List<PlayerDialogueOption>)EditorGUILayout.ObjectField(dialogueOption.playerResponses, typeof(List<PlayerDialogueOption>));

//        }
//        else
//        {

//        }

//        GUILayout.Toggle(dialogueOption.limitedTime, "LimitedTime");

//        if (dialogueOption.limitedTime)
//        {
            
//        }

//    }
//}
//#endif