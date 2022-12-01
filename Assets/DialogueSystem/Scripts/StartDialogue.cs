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
        dialogueSystem.npcNameText.text = npcInfo.npcName;


        if (FindObjectOfType<PlayerInteractionRaycast>().selectedObject.GetComponent<NPCBrain>() && FindObjectOfType<PlayerInteractionRaycast>().selectedObject.GetComponent<NPCBrain>().npcInfo == npcInfo)
        {
            NPCBrain currentNPC = FindObjectOfType<PlayerInteractionRaycast>().selectedObject.GetComponent<NPCBrain>();

            List<NPCDialogueOption> usedDialogue = new List<NPCDialogueOption>();

            foreach (NPCBrain.DialogueMemory dialogueMemory in currentNPC.dialogueMemories)
            {
                usedDialogue.Add(dialogueMemory.npcUsedDialogue);
            }

            if (currentNPC.startingDialogue != null && !usedDialogue.Contains(currentNPC.startingDialogue))
            {
                NPCInitiatedDialogue(npcInfo, currentNPC.startingDialogue);
                return;
            }
        }

        NPCDialogueOption greetingDialogue = npcInfo.npcDialogue.greetingDialogue[Random.Range(0, npcInfo.npcDialogue.greetingDialogue.Count)];

        if (npcInfo.npcDialogue.dialogueConnections.Count > 0 /*&& !greetingDialogue.isNPCInitatied*/) //if the player has stuff to say to the npc
        {
            playerDialogue.AddDialogueOptions();
        }
        //else if (greetingDialogue.isNPCInitatied)
        //{
            
        //    for (int i = 0; i < npcInfo.npcDialogue.dialogueConnections.Count; i++)
        //    {
        //        if (npcInfo.npcDialogue.dialogueConnections[i].playerDialogueInput == null && npcInfo.npcDialogue.dialogueConnections[i].npcResponses[i].response.requiresResponse || npcInfo.npcDialogue.dialogueConnections[i].npcResponses[i].response.toOtherNPC)
        //        {
        //            //figure out a new way of providing specific start dialogue 

        //            NPCInitiatedDialogue(npcInfo, npcInfo.npcDialogue.dialogueConnections[i].npcResponses[i].response);

        //            return;
        //        }
        //    }
           
        //}

        greetingDialogue.playerResponses = playerDialogue.SetPlayerDialogueBasedOnCurrentNPCAndDialogue(npcInfo, greetingDialogue).playerResponses;


        //dialogueSystem.playerDialogueText.text = playerDialogue.greetingDialogue[Random.Range(0, playerDialogue.greetingDialogue.Count)].dialogue;

        //Deactivate Player Controller
        FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;



        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        //Debug.Log("Cursor Mode Confined");

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


            dialogueSystem.playerMovement.enabled = false;

            //Lock Camera to NPC target

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            //playerDialogue.AddResponseOptions();



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
