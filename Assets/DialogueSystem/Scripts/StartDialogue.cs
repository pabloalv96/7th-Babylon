using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogue : MonoBehaviour
{
    public DialogueListSystem dialogueSystem;
    public PlayerDialogue playerDialogue;

    private void Start()
    {
        //dialogueSystem = FindObjectOfType<NEWListDialogueSystem>();
        //playerDialogue = FindObjectOfType<PlayerDialogue>();
    }
    public void EnterDialogue( NPCInfo npcInfo)
    {
        //Set NPC
        //Set greeting text
        //Set player options
        dialogueSystem.inDialogue = true;

        dialogueSystem.enabled = true;

        dialogueSystem.npc = npcInfo;
        //dialogueSystem.npc.npcEmotions.SetMood();
        dialogueSystem.npcNameText.text = npcInfo.npcName + ":";

        NPCDialogueOption greetingDialogue = npcInfo.npcDialogue.greetingDialogue[Random.Range(0, npcInfo.npcDialogue.greetingDialogue.Count)];

        if (npcInfo.npcDialogue.dialogueConnections.Count > 0)
        {
            playerDialogue.AddDialogueOptions();
        }
        //else if (npcInfo.npcDialogue.dialogueConnections[0].playerDialogueInput == null && npcInfo.npcDialogue.dialogueConnections[0].npcResponses[0].response.requiresResponse)
        //{
        //    NPCInitiatedDialogue(npcInfo, npcInfo.npcDialogue.dialogueConnections[0].npcResponses[0].response);

        //    playerDialogue.AddResponseOptions();

        //    return;
        //}        //else if (npcInfo.npcDialogue.dialogueConnections[0].playerDialogueInput == null && npcInfo.npcDialogue.dialogueConnections[0].npcResponses[0].response.requiresResponse)
        //{
        //    NPCInitiatedDialogue(npcInfo, npcInfo.npcDialogue.dialogueConnections[0].npcResponses[0].response);

        //    playerDialogue.AddResponseOptions();

        //    return;
        //}

        greetingDialogue.playerResponses = playerDialogue.SetPlayerDialogueBasedOnCurrentNPCAndDialogue(npcInfo, greetingDialogue).playerResponses;
        //dialogueSystem.ListDialogueOptions();


        //dialogueSystem.playerDialogueText.text = playerDialogue.greetingDialogue[Random.Range(0, playerDialogue.greetingDialogue.Count)].dialogue;

        //Deactivate Player Controller
        FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;



        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Debug.Log("Cursor Mode Confined");

        dialogueSystem.playerIsLeading = true;

        dialogueSystem.SetNewDialogueText(greetingDialogue);
        dialogueSystem.BeginDialogue();


    }

    public void NPCInitiatedDialogue(NPCInfo npc, NPCDialogueOption startingDialogue)
    {
        if (startingDialogue.toOtherNPC)
        {
            BeginSubtitleSequence(npc, startingDialogue);
        }
        else
        {
            //Set NPC
            //Set greeting text
            //Set player options
            dialogueSystem.inDialogue = true;

            dialogueSystem.enabled = true;



            dialogueSystem.npc = npc;

            //dialogueSystem.npc.npcEmotions.SetMood();
            dialogueSystem.npcNameText.text = npc.npcName;

            //if (npc.npcDialogue.dialogueConnections.Count > 0)
            //{
            //    playerDialogue.AddResponseOptions();
            //}

            //startingDialogue.playerResponses = playerDialogue.SetPlayerDialogueBasedOnCurrentNPCAndDialogue(npc, startingDialogue).playerResponses;

            //playerDialogue.AddResponseOptions();

            dialogueSystem.playerMovement.enabled = false;

            //Lock Camera to NPC target
            //dialogueSystem.playerDialogueText.text = playerDialogue.continueDialogue.dialogue;

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            

            dialogueSystem.playerIsLeading = false;

            dialogueSystem.SetNewDialogueText(startingDialogue);
            dialogueSystem.BeginDialogue();
        }
        
    }

    public void BeginSubtitleSequence(NPCInfo npc, NPCDialogueOption startingDialogue)
    {
        dialogueSystem.enabled = true;

        dialogueSystem.SetNewDialogueText(startingDialogue);

        dialogueSystem.inDialogue = false;

        dialogueSystem.npc = npc;

        dialogueSystem.npcNameText.text = npc.npcName;

        dialogueSystem.playerMovement.enabled = true;


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

        dialogueSystem.BeginDialogue();

    }

    public void EnterDialogueWithRandomNPC()
    {
        EnterDialogue(GetRandomNPC());
    }

    public NPCInfo GetRandomNPC()
    {
        int rand = Random.Range(0, playerDialogue.playerQuestions.Count);

        NPCInfo npc = playerDialogue.playerQuestions[rand].npc;

        return npc;
    }
}
