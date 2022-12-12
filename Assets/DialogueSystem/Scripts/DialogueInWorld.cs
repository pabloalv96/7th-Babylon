using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInWorld : MonoBehaviour
{ 
    public NPCDialogueOption dialogueFromWorld;
    public NPCInfo narrator;


    private void Awake()
    {


        PlayerDialogue.PlayerQuestions newPlayerQuestions = new PlayerDialogue.PlayerQuestions();

        newPlayerQuestions.npc = narrator;

        //foreach (PlayerDialogueOption response in dialogueFromWorld.playerResponses)
        //{
        //    PlayerDialogueOption newResponse = response;
        //    newPlayerQuestions.questionsForNPC.Add(newResponse);
        //}

        //Debug.Log("Generating questions for: " + npcInfoList[i].npcName);


        FindObjectOfType<PlayerDialogue>().playerQuestions.Add(newPlayerQuestions);
        
    }
}
