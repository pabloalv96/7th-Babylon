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
        dialogueSystem.enabled = true;

        dialogueSystem.npc = npcInfo;
        //dialogueSystem.npc.npcEmotions.SetMood();
        dialogueSystem.npcNameText.text = npcInfo.npcName + ":";

        NPCDialogueOption greetingDialogue = npcInfo.npcDialogue.greetingDialogue[Random.Range(0, npcInfo.npcDialogue.greetingDialogue.Count)];

        if (npcInfo.npcDialogue.dialogueConnections.Count > 0)
        {
            playerDialogue.AddDialogueOptions();
        }

        greetingDialogue.playerResponses = playerDialogue.SetPlayerDialogueBasedOnCurrentNPCAndDialogue(npcInfo, greetingDialogue).playerResponses;
        //dialogueSystem.ListDialogueOptions();


        dialogueSystem.playerDialogueText.text = playerDialogue.greetingDialogue[Random.Range(0, playerDialogue.greetingDialogue.Count)].dialogue;

        //Deactivate Player Controller
        dialogueSystem.playerMovement.enabled = false;
        //dialogueSystem.playerCam.enabled = false;

        //Lock Camera to NPC target


        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        dialogueSystem.BeginDialogue();

        dialogueSystem.SetNewDialogueText(greetingDialogue);

    }

    public void NPCInitiatedDialogue(NPCInfo npc, NPCDialogueOption startingDialogue)
    {
        //Set NPC
        //Set greeting text
        //Set player options
        dialogueSystem.inDialogue = true;
        dialogueSystem.enabled = true;

        dialogueSystem.npc = npc;

        //dialogueSystem.npc.npcEmotions.SetMood();
        dialogueSystem.npcNameText.text = npc.npcName + ":";

        //if (npc.npcDialogue.dialogueConnections.Count > 0)
        //{
        //    playerDialogue.AddResponseOptions();
        //}

        //startingDialogue.playerResponses = playerDialogue.SetPlayerDialogueBasedOnCurrentNPCAndDialogue(npc, startingDialogue).playerResponses;

        //playerDialogue.AddResponseOptions();

        dialogueSystem.playerMovement.enabled = false;

        //Lock Camera to NPC target
        dialogueSystem.playerDialogueText.text = playerDialogue.continueDialogue.dialogue;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        dialogueSystem.BeginDialogue();

        dialogueSystem.SetNewDialogueText(startingDialogue);
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
